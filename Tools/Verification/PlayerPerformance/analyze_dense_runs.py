#!/usr/bin/env python3
"""Turn the probe's report.json files (and the sampled machine data) into plain tables.

    python3 Tools/Verification/PlayerPerformance/analyze_dense_runs.py Build/PlayerPerformance/D01/S1 > summary.md

Reads every <plan>-<res>/ folder under the series: report.json (DensePerfProbe), gpu.csv (nvidia-smi), threads.json (per-thread CPU).
Nothing here changes the data; it only summarises it.
"""
import csv
import json
import statistics
import sys
from pathlib import Path


def fmt(x, digits=2):
    return "-" if x is None else f"{x:.{digits}f}"


def series(window, name):
    for s in window.get("series", []):
        if s["name"] == name or s["name"].endswith("/" + name):
            return s
    return None


def mean_of(window, name):
    s = series(window, name)
    return s["mean"] if s else None


def gpu_valid(run, window_name):
    """GPU frame time over the frames that carry a value (the engine reports 0 for frames without one): (mean, median, valid count, total)."""
    path = run / (window_name + "-frames.csv")
    if not path.exists():
        return None
    with path.open() as f:
        reader = csv.DictReader(f)
        values = []
        total = 0
        for row in reader:
            total += 1
            try:
                v = float(row["gpu_ms"])
            except (ValueError, KeyError):
                continue
            if v > 0:
                values.append(v)
    if not values:
        return (None, None, 0, total)
    return (statistics.mean(values), statistics.median(values), len(values), total)


def window_table(report, with_counters=True, run=None):
    rows = ["| window | view | toggles | marks | frames | mean ms | median | p95 | p99 | max | fps (median) | main ms | render ms | gpu ms (valid frames) | alloc KB/frame | gc0 | focus |",
            "|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|"]
    for w in report["windows"]:
        alloc = mean_of(w, "GC Allocated In Frame")
        rows.append("| {name} | {view} | {tog} | {marks} | {frames} | {mean} | {med} | {p95} | {p99} | {mx} | {fps} | {main} | {render} | {gpu} | {alloc} | {gc} | {focus} |".format(
            name=w["name"], view=w["view"], tog=w["toggles"] or "-", marks=w["marks"], frames=w["frames"], mean=fmt(w["meanMs"]), med=fmt(w["medianMs"]), p95=fmt(w["p95Ms"]),
            p99=fmt(w["p99Ms"]), mx=fmt(w["maxMs"]), fps=fmt(1000 / w["medianMs"], 1) if w["medianMs"] else "-",
            main=fmt(mean_of(w, "cpuMainThreadFrameTime_ms")), render=fmt(mean_of(w, "cpuRenderThreadFrameTime_ms")), gpu=gpu_text(run, w),
            alloc=fmt(alloc / 1024 if alloc is not None else None, 1), gc=w["gc0Delta"], focus=fmt(w["focusedFraction"], 3)))
    if with_counters:
        names = sorted({s["name"] for w in report["windows"] for s in w["series"] if s["name"].split("/")[0] in ("Render", "Memory", "Internal", "Scripts", "PlayerLoop", "VSync")})
        if names:
            rows += ["", "| window | " + " | ".join(n.split("/", 1)[1] for n in names) + " |", "|---|" + "---|" * len(names)]
            for w in report["windows"]:
                cells = []
                for n in names:
                    s = series(w, n)
                    cells.append(fmt(s["mean"], 1) if s else "-")
                rows.append(f"| {w['name']} | " + " | ".join(cells) + " |")
    return "\n".join(rows)


def gpu_text(run, w):
    g = gpu_valid(run, w["name"]) if run is not None else None
    if g is None:
        return fmt(mean_of(w, "gpuFrameTime_ms"))
    return "none valid" if g[0] is None else f"{g[0]:.1f} ({g[2]}/{g[3]})"


def hitch_table(report):
    rows = ["| window | frame | at s | ms | gc alloc KB | gc0 in frame |", "|---|---|---|---|---|---|"]
    count = 0
    for w in report["windows"]:
        for h in w["hitches"]:
            rows.append(f"| {w['name']} | {h['frame']} | {h['atSeconds']:.1f} | {h['ms']:.1f} | {h['gcAllocBytes'] / 1024:.1f} | {h['gc0Delta']} |")
            count += 1
    return "\n".join(rows) if count else "no hitches over max(3x median, median+20 ms)"


def timing_table(report):
    rows = ["| group | name | n | mean ms | median | p95 | min | max | mono KB/call | note |", "|---|---|---|---|---|---|---|---|---|---|"]
    for t in report["timings"]:
        rows.append(f"| {t['group']} | {t['name']} | {t['n']} | {fmt(t['meanMs'])} | {fmt(t['medianMs'])} | {fmt(t['p95Ms'])} | {fmt(t['minMs'])} | {fmt(t['maxMs'])} | {t['monoBytesPerCall'] / 1024:.1f} | {t['note']} |")
    return "\n".join(rows)


def memory_table(report):
    rows = ["| phase | working set GiB | private GiB | mono MB | unity alloc MB | unity reserved MB | gfx driver MB | gc0 |", "|---|---|---|---|---|---|---|---|"]
    for m in report["memory"]:
        rows.append(f"| {m['phase']} | {m['workingSet'] / 2**30:.2f} | {m['privateBytes'] / 2**30:.2f} | {m['monoUsed'] / 2**20:.0f} | {m['unityAllocated'] / 2**20:.0f} | {m['unityReserved'] / 2**20:.0f} | {m['graphicsDriver'] / 2**20:.0f} | {m['gc0']} |")
    return "\n".join(rows)


def linear_fit(xs, ys):
    n = len(xs)
    mx, my = sum(xs) / n, sum(ys) / n
    sxx = sum((x - mx) ** 2 for x in xs)
    if sxx == 0:
        return 0, my, 0
    slope = sum((x - mx) * (y - my) for x, y in zip(xs, ys)) / sxx
    intercept = my - slope * mx
    ss_tot = sum((y - my) ** 2 for y in ys)
    ss_res = sum((y - (intercept + slope * x)) ** 2 for x, y in zip(xs, ys))
    return slope, intercept, (1 - ss_res / ss_tot) if ss_tot else 1


def marks_scaling(report):
    fell = [(w["marks"], w["medianMs"], w["meanMs"], mean_of(w, "cpuMainThreadFrameTime_ms"), mean_of(w, "GC Allocated In Frame")) for w in report["windows"] if w["name"].startswith("marks_fell_")]
    if len(fell) < 3:
        return "(no marks scaling windows)"
    out = ["| fell marks | median frame ms | mean frame ms | main thread ms | alloc KB/frame |", "|---|---|---|---|---|"]
    for n, med, mean, main, alloc in sorted(fell):
        out.append(f"| {n} | {fmt(med)} | {fmt(mean)} | {fmt(main)} | {fmt(alloc / 1024 if alloc is not None else None, 1)} |")
    xs = [f[0] for f in fell]
    ys = [f[2] for f in fell]
    slope, intercept, r2 = linear_fit(xs, ys)
    out.append(f"\nlinear fit of mean frame ms against marks: {slope:.3f} ms per mark, intercept {intercept:.2f} ms, R^2 {r2:.3f}")
    # A quadratic term would show as clearly better fit with x^2; report the ratio of the last-step slope to the first-step slope as a simple shape check.
    srt = sorted(fell)
    if len(srt) >= 4:
        first = (srt[1][2] - srt[0][2]) / max(1, srt[1][0] - srt[0][0])
        last = (srt[-1][2] - srt[-2][2]) / max(1, srt[-1][0] - srt[-2][0])
        out.append(f"incremental cost per mark between the two smallest counts: {first:.3f} ms; between the two largest: {last:.3f} ms")
    return "\n".join(out)


def ab_table(report, run=None):
    windows = {w["name"]: w for w in report["windows"]}
    bases = [w for w in report["windows"] if w["name"].startswith("ab_base")]
    if not bases:
        return "(no A/B windows)"
    base_mean = statistics.mean(w["medianMs"] for w in bases)
    spread = max(w["medianMs"] for w in bases) - min(w["medianMs"] for w in bases)
    out = [f"baseline windows (median frame ms): " + ", ".join(f"{w['name']}={w['medianMs']:.2f}" for w in bases) + f"; mean {base_mean:.2f}, spread {spread:.2f} ms",
           "", "| condition | median ms | mean ms | p95 | delta vs baseline mean | % | gpu ms | main ms | draw calls | triangles | verified |", "|---|---|---|---|---|---|---|---|---|---|---|"]
    for w in report["windows"]:
        if not w["name"].startswith("ab_") or w["name"].startswith("ab_base"):
            continue
        delta = w["medianMs"] - base_mean
        out.append(f"| {w['name'][3:]} | {fmt(w['medianMs'])} | {fmt(w['meanMs'])} | {fmt(w['p95Ms'])} | {delta:+.2f} | {100 * delta / base_mean:+.1f} | {gpu_text(run, w)} | "
                   f"{fmt(mean_of(w, 'cpuMainThreadFrameTime_ms'))} | {fmt(mean_of(w, 'Draw Calls Count'), 0)} | {fmt(mean_of(w, 'Triangles Count'), 0)} | {'beyond drift' if abs(delta) > spread else 'within drift'} |")
    return "\n".join(out)


def machine(run):
    lines = []
    gpu = run / "gpu.csv"
    if gpu.exists():
        with gpu.open() as f:
            reader = csv.reader(f)
            next(reader, None)
            rows = [r for r in reader if len(r) >= 10]
        def col(i):
            vals = []
            for r in rows:
                try:
                    vals.append(float(r[i].split()[0]))
                except (ValueError, IndexError):
                    pass
            return vals
        if rows:
            util, sm, temp, power = col(5), col(1), col(4), col(7)
            lines.append(f"GPU (nvidia-smi, {len(rows)} samples): utilisation mean {statistics.mean(util):.0f}% max {max(util):.0f}%; SM clock mean {statistics.mean(sm):.0f} MHz (max seen {max(sm):.0f}); "
                         f"temperature {min(temp):.0f}-{max(temp):.0f} C; power mean {statistics.mean(power):.0f} W; throttle reasons seen: {sorted({r[9].strip() for r in rows})}")
    threads = run / "threads.json"
    if threads.exists():
        samples = json.loads(threads.read_text())
        totals = {}
        for s in samples:
            for name, v in s.get("threads_cpu_seconds_per_second", {}).items():
                totals.setdefault(name, []).append(v)
        top = sorted(((statistics.mean(v), k) for k, v in totals.items() if len(v) > 3), reverse=True)[:8]
        if top:
            lines.append("busiest Player threads (CPU seconds per second, mean over samples where active): " + "; ".join(f"{k.split(':')[0]}={v:.2f}" for v, k in top))
        loads = [float(s["loadavg"][0]) for s in samples if s.get("loadavg")]
        if loads:
            lines.append(f"machine load average (1 min) during run: {min(loads):.2f}-{max(loads):.2f}")
    return "\n\n".join(lines)


def main():
    series_dir = Path(sys.argv[1])
    for run in sorted(p for p in series_dir.iterdir() if p.is_dir() and (p / "report.json").exists()):
        report = json.loads((run / "report.json").read_text())
        print(f"\n## {run.name}  ({report['status']})\n")
        print(f"{report['gpu']} / {report['graphicsApi']} / {report['unity']} / development={report['developmentBuild']} / {report['width']}x{report['height']} / quality {report['quality']} / "
              f"vSync {report['vSync']} / targetFrameRate {report['targetFrameRate']} / lodBias {report['lodBias']} / shadowDistance {report['shadowDistance']} / trees {report['treeCount']} / cells {report['cells']}\n")
        if report.get("markerNotes"):
            print(f"marker notes: {report['markerNotes']}\n")
        if report["windows"]:
            print("### frame windows\n\n" + window_table(report, run=run) + "\n")
            print("### hitches\n\n" + hitch_table(report) + "\n")
            if any(w["name"].startswith("marks_fell_") for w in report["windows"]):
                print("### marks scaling\n\n" + marks_scaling(report) + "\n")
            if any(w["name"].startswith("ab_") for w in report["windows"]):
                print("### render A/B\n\n" + ab_table(report, run) + "\n")
        if report["timings"]:
            print("### timed operations\n\n" + timing_table(report) + "\n")
        if report["memory"]:
            print("### memory\n\n" + memory_table(report) + "\n")
        info = machine(run)
        if info:
            print("### machine\n\n" + info + "\n")


if __name__ == "__main__":
    main()
