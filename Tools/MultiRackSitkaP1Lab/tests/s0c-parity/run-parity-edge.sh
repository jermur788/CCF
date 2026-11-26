#!/usr/bin/env bash
# S0c real-browser parity driver (SPEC-multi-2d-s0-seam-extraction-V1 §7.5).
#
# Runs 9 scripted UI states across the full 4-cell matrix:
#   {ma-1, ma-1-mixed} x {1280x720, 1920x1080} = 36 states
# in installed Microsoft Edge (--headless=new, DSF 1) against real WSL2 sitkap1-ui processes.
#
# Modes:
#   run-parity-edge.sh --mode reference <ref-dir>
#       Captures two consecutive runs on the current build, verifies self-determinism (runA === runB
#       outerHTML across all 36 states), and writes the verified reference manifest + artifacts to <ref-dir>.
#   run-parity-edge.sh --mode compare --ref <ref-dir> <evidence-dir>
#       Captures the current build and compares every state against <ref-dir>:
#       - Primary oracle: canonicalized document.body.outerHTML must be 100% byte-identical for all 36 states.
#       - Secondary oracle: PNG screenshots compared pixel-by-pixel; any differing state gets a diff report
#         (and diff PNG when Python Pillow is available or raw-RGBA diff summary via zlib/struct stdlib).
#       - Zero non-loopback requests, zero console errors, zero CSP violations, zero 4xx/5xx.
#   run-parity-edge.sh <evidence-dir>
#       Single-argument gate invocation (used by build/2d-gate.sh run_stage_B):
#       - If S0C_PARITY_REF is set and contains manifest.json, delegates to --mode compare --ref "$S0C_PARITY_REF".
#       - Otherwise runs two consecutive deterministic renders (runA vs runB) on the current build, verifies
#         100% outerHTML identity across all 36 states + zero non-loopback/console/CSP/HTTP errors.
set -euo pipefail

p1_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd -P)"
repo="$(git -C "$p1_root" rev-parse --show-toplevel)"
dotnet_bin="${DOTNET:-$HOME/.dotnet/dotnet}"
node_win="${NODE_WIN:-/mnt/c/Program Files/nodejs/node.exe}"
ui_dll="$p1_root/src/Ui/bin/Debug/net8.0/sitkap1-ui.dll"
fixture_name="p1d-30m-sitka-adult-v1.sitkap1fx.json"

MODE="auto"
REF_DIR=""
OUT_DIR=""

usage() {
  cat >&2 <<'EOF'
Usage:
  tests/s0c-parity/run-parity-edge.sh <evidence-dir>
  tests/s0c-parity/run-parity-edge.sh --mode reference <ref-dir>
  tests/s0c-parity/run-parity-edge.sh --mode compare --ref <ref-dir> <evidence-dir>
EOF
  exit 64
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --mode)
      [[ $# -ge 2 ]] || usage
      MODE="$2"
      shift 2
      ;;
    --ref)
      [[ $# -ge 2 ]] || usage
      REF_DIR="$2"
      shift 2
      ;;
    --help|-h)
      usage
      ;;
    -*)
      echo "USAGE ERROR: unknown option: $1" >&2
      usage
      ;;
    *)
      [[ -z "$OUT_DIR" ]] || usage
      OUT_DIR="$1"
      shift
      ;;
  esac
done

[[ -n "$OUT_DIR" ]] || usage
if [[ "$MODE" == "auto" ]]; then
  if [[ -n "${S0C_PARITY_REF:-}" && -f "${S0C_PARITY_REF}/manifest.json" ]]; then
    MODE="compare"
    REF_DIR="$S0C_PARITY_REF"
  else
    MODE="selfcheck"
  fi
fi

case "$MODE" in
  reference|selfcheck) ;;
  compare)
    [[ -n "$REF_DIR" && -f "$REF_DIR/manifest.json" ]] || { echo "STOP: --mode compare requires --ref <dir> containing manifest.json" >&2; exit 2; }
    ;;
  *)
    echo "USAGE ERROR: invalid --mode $MODE" >&2
    usage
    ;;
esac

[[ -x "$dotnet_bin" ]] || { echo "STOP: $dotnet_bin is not executable (set DOTNET)" >&2; exit 2; }
[[ -x "$node_win" ]] || { echo "STOP: Windows node not found at $node_win (set NODE_WIN)" >&2; exit 2; }
[[ -f "$ui_dll" ]] || { echo "STOP: build first: $dotnet_bin build $p1_root/src/Ui/SitkaP1.Ui.csproj" >&2; exit 2; }
[[ -f "$p1_root/fixtures/derived/$fixture_name" ]] || { echo "STOP: fixture missing" >&2; exit 2; }

mkdir -p -- "$OUT_DIR"
OUT_DIR="$(cd "$OUT_DIR" && pwd -P)"
case "$OUT_DIR/" in "$repo"/*) echo "STOP: evidence dir is inside repository" >&2; exit 2 ;; esac

scratch="$(mktemp -d "${TMPDIR:-/tmp}/ccf-s0c-parity.XXXXXXXX")"
case "$(cd "$scratch" && pwd -P)/" in "$repo"/*) echo "STOP: TMPDIR inside repository" >&2; rm -rf -- "$scratch"; exit 2 ;; esac
mkdir -p "$scratch/fixture"
cp -- "$p1_root/fixtures/derived/$fixture_name" "$scratch/fixture/$fixture_name"

log="$OUT_DIR/parity.log"
: > "$log"
say() { printf '%s\n' "$*" | tee -a "$log"; }

driver_win="$(wslpath -w "$p1_root/tests/s0c-parity/drive-parity.mjs")"

server_pid=""
server_url=""
cleanup() {
  if [[ -n "$server_pid" ]] && kill -0 "$server_pid" 2>/dev/null; then
    kill -KILL -"$server_pid" 2>/dev/null || kill -KILL "$server_pid" 2>/dev/null || true
  fi
  rm -rf -- "$scratch"
}
trap cleanup EXIT

start_server() {
  local tag="$1" exp="$2" xdg="$3"
  mkdir -p "$xdg"
  set -m
  ( cd "$p1_root" && exec env XDG_DATA_HOME="$xdg" "$dotnet_bin" "$ui_dll" "$scratch/fixture" \
      --multi-experiment="$exp" ) > "$scratch/server-${tag}.out" 2> "$scratch/server-${tag}.err" &
  server_pid=$!
  set +m
  server_url=""
  for _ in $(seq 1 300); do
    server_url="$(grep -oE 'http://127\.0\.0\.1:[0-9]+/s/[0-9a-f]{64}/' "$scratch/server-${tag}.out" | head -1 || true)"
    [[ -n "$server_url" ]] && break
    kill -0 "$server_pid" 2>/dev/null || break
    sleep 0.1
  done
  if [[ -z "$server_url" ]]; then
    say "FAIL server-${tag} printed no launch URL"
    cat "$scratch/server-${tag}.err" | tee -a "$log"
    exit 1
  fi
}

stop_server() {
  local tag="$1" rc=0
  kill -INT "$server_pid" 2>/dev/null || true
  for _ in $(seq 1 150); do kill -0 "$server_pid" 2>/dev/null || break; sleep 0.1; done
  if kill -0 "$server_pid" 2>/dev/null; then
    say "FAIL server-${tag} did not stop on SIGINT"
    kill -KILL "$server_pid" 2>/dev/null || true
    return 1
  fi
  wait "$server_pid" 2>/dev/null || rc=$?
  server_pid=""
  if [[ "$rc" -ne 0 ]]; then
    say "FAIL server-${tag} exit ${rc}"
    return 1
  fi
  if [[ -s "$scratch/server-${tag}.err" ]]; then
    say "FAIL server-${tag} wrote to stderr"
    cat "$scratch/server-${tag}.err" | tee -a "$log"
    return 1
  fi
  return 0
}

capture_matrix() {
  local label="$1" target_dir="$2"
  mkdir -p -- "$target_dir"
  local target_win
  target_win="$(wslpath -w "$target_dir")"
  local cell exp dims w h xdg
  local cells=(
    "ma-1:1280:720"
    "ma-1:1920:1080"
    "ma-1-mixed:1280:720"
    "ma-1-mixed:1920:1080"
  )
  for spec in "${cells[@]}"; do
    IFS=':' read -r exp w h <<< "$spec"
    cell="${exp}_${w}x${h}"
    xdg="$scratch/xdg-${label}-${cell}"
    say "=== CAPTURE [${label}] ${cell} ==="
    start_server "${label}-${cell}" "$exp" "$xdg"
    local rc=0
    "$node_win" "$driver_win" --url "${server_url%/}/multi" --out "$target_win" --cell "$cell" --width "$w" --height "$h" \
      2>&1 | sed -E 's#/s/[0-9a-f]{64}/#/s/<capability>/#g' | tee -a "$log" || rc=$?
    stop_server "${label}-${cell}" || rc=1
    if [[ "$rc" -ne 0 ]]; then
      say "FAIL capture ${label} ${cell} (rc=${rc})"
      return 1
    fi
  done

  # Collate cell manifests into one top-level manifest.json
  python3 - "$target_dir" "$repo" "$label" <<'PY'
import datetime, glob, json, os, subprocess, sys

tdir, repo, label = sys.argv[1:]
head = subprocess.check_output(["git", "-C", repo, "rev-parse", "HEAD"], text=True).strip()
cells = {}
for p in sorted(glob.glob(os.path.join(tdir, "*__manifest.json"))):
    doc = json.load(open(p, "r", encoding="utf-8"))
    cells[doc["cell"]] = doc
manifest = {
    "schemaVersion": 1,
    "label": label,
    "capturedUtc": datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
    "headCommit": head,
    "cellCount": len(cells),
    "totalStates": sum(len(c["states"]) for c in cells.values()),
    "cells": cells,
}
with open(os.path.join(tdir, "manifest.json"), "w", encoding="utf-8") as f:
    json.dump(manifest, f, indent=2)
    f.write("\n")
PY
}

compare_manifests() {
  local dir_a="$1" dir_b="$2" report_json="$3" diff_dir="$4"
  mkdir -p -- "$diff_dir"
  python3 - "$dir_a" "$dir_b" "$report_json" "$diff_dir" <<'PY' | tee -a "$log"
import difflib, json, os, struct, sys, zlib

dir_a, dir_b, report_path, diff_dir = sys.argv[1:]
ma = json.load(open(os.path.join(dir_a, "manifest.json"), "r", encoding="utf-8"))
mb = json.load(open(os.path.join(dir_b, "manifest.json"), "r", encoding="utf-8"))

def decode_png_rgba(raw):
    if raw[:8] != b"\x89PNG\r\n\x1a\n":
        raise ValueError("not a PNG")
    pos = 8
    w = h = bit_depth = color_type = None
    idat = bytearray()
    while pos < len(raw):
        length = struct.unpack(">I", raw[pos:pos+4])[0]
        ctype = raw[pos+4:pos+8]
        cdata = raw[pos+8:pos+8+length]
        pos += 12 + length
        if ctype == b"IHDR":
            w, h, bit_depth, color_type, comp, filt, interlace = struct.unpack(">IIBBBBB", cdata)
            if bit_depth != 8 or comp != 0 or filt != 0 or interlace != 0:
                raise ValueError(f"unsupported PNG header: depth={bit_depth} interlace={interlace}")
        elif ctype == b"IDAT":
            idat.extend(cdata)
        elif ctype == b"IEND":
            break
    bpp = {2: 3, 6: 4}[color_type]
    raw_scan = zlib.decompress(bytes(idat))
    stride = w * bpp
    out = bytearray(h * stride)
    in_pos = 0
    for y in range(h):
        ftype = raw_scan[in_pos]
        in_pos += 1
        row_start = y * stride
        for x in range(stride):
            val = raw_scan[in_pos + x]
            a = out[row_start + x - bpp] if x >= bpp else 0
            b = out[row_start - stride + x] if y > 0 else 0
            c = out[row_start - stride + x - bpp] if (y > 0 and x >= bpp) else 0
            if ftype == 0:
                recon = val
            elif ftype == 1:
                recon = (val + a) & 0xFF
            elif ftype == 2:
                recon = (val + b) & 0xFF
            elif ftype == 3:
                recon = (val + ((a + b) >> 1)) & 0xFF
            elif ftype == 4:
                p = a + b - c
                pa, pb, pc = abs(p - a), abs(p - b), abs(p - c)
                pr = a if (pa <= pb and pa <= pc) else (b if pb <= pc else c)
                recon = (val + pr) & 0xFF
            else:
                raise ValueError(f"unknown PNG filter {ftype}")
            out[row_start + x] = recon
        in_pos += stride
    return w, h, bpp, out

def encode_png_rgb(w, h, rgb_bytes):
    def chunk(ctype, data):
        return struct.pack(">I", len(data)) + ctype + data + struct.pack(">I", zlib.crc32(ctype + data) & 0xFFFFFFFF)
    ihdr = struct.pack(">IIBBBBB", w, h, 8, 2, 0, 0, 0)
    raw = bytearray()
    stride = w * 3
    for y in range(h):
        raw.append(0)
        raw.extend(rgb_bytes[y*stride:(y+1)*stride])
    return b"\x89PNG\r\n\x1a\n" + chunk(b"IHDR", ihdr) + chunk(b"IDAT", zlib.compress(bytes(raw), 6)) + chunk(b"IEND", b"")

cells_a = sorted(ma["cells"].keys())
cells_b = sorted(mb["cells"].keys())
if cells_a != cells_b:
    print(f"FAIL cell set mismatch: A={cells_a} B={cells_b}")
    sys.exit(1)

html_pass = 0
html_fail = 0
png_identical = 0
png_diff = 0
rows = []

for cell in cells_a:
    ca, cb = ma["cells"][cell], mb["cells"][cell]
    if cb["nonLoopbackCount"] != 0 or cb["consoleErrorCount"] != 0 or cb["cspViolationCount"] != 0 or cb["httpErrorCount"] != 0:
        print(f"FAIL {cell}: diagnostics non-zero in candidate")
        sys.exit(1)
    sa_map = {s["state"]: s for s in ca["states"]}
    sb_map = {s["state"]: s for s in cb["states"]}
    if sorted(sa_map.keys()) != sorted(sb_map.keys()):
        print(f"FAIL {cell}: state keys mismatch")
        sys.exit(1)
    for st in sorted(sa_map.keys()):
        sa, sb = sa_map[st], sb_map[st]
        ha = open(os.path.join(dir_a, sa["htmlFile"]), "rb").read()
        hb = open(os.path.join(dir_b, sb["htmlFile"]), "rb").read()
        html_eq = (ha == hb and sa["htmlSha256"] == sb["htmlSha256"])
        if html_eq:
            html_pass += 1
        else:
            html_fail += 1
            diff_lines = list(difflib.unified_diff(
                ha.decode("utf-8", errors="replace").splitlines(),
                hb.decode("utf-8", errors="replace").splitlines(),
                fromfile="ref/" + sa["htmlFile"], tofile="cand/" + sb["htmlFile"], lineterm=""
            ))
            open(os.path.join(diff_dir, f"{cell}__{st}.html.diff"), "w", encoding="utf-8").write("\n".join(diff_lines[:200]) + "\n")
            print(f"FAIL HTML DIFF {cell} {st}: ref={sa['htmlSha256'][:12]} cand={sb['htmlSha256'][:12]}")

        pa = open(os.path.join(dir_a, sa["pngFile"]), "rb").read()
        pb = open(os.path.join(dir_b, sb["pngFile"]), "rb").read()
        png_eq = (pa == pb)
        pixel_info = {"byteIdentical": png_eq, "differingPixels": 0, "totalPixels": 0, "bbox": None, "diffImage": None}
        if png_eq:
            png_identical += 1
        else:
            wa, ha_px, bppa, pix_a = decode_png_rgba(pa)
            wb, hb_px, bppb, pix_b = decode_png_rgba(pb)
            if (wa, ha_px) != (wb, hb_px):
                png_diff += 1
                pixel_info["sizeMismatch"] = [[wa, ha_px], [wb, hb_px]]
            else:
                diff_px = 0
                min_x, min_y, max_x, max_y = wa, ha_px, -1, -1
                diff_rgb = bytearray(wa * ha_px * 3)
                for y in range(ha_px):
                    for x in range(wa):
                        ia = (y * wa + x) * bppa
                        ib = (y * wa + x) * bppb
                        ra, ga, ba = pix_a[ia], pix_a[ia+1], pix_a[ia+2]
                        rb, gb, bb = pix_b[ib], pix_b[ib+1], pix_b[ib+2]
                        o = (y * wa + x) * 3
                        if ra != rb or ga != gb or ba != bb:
                            diff_px += 1
                            if x < min_x: min_x = x
                            if y < min_y: min_y = y
                            if x > max_x: max_x = x
                            if y > max_y: max_y = y
                            diff_rgb[o] = 255
                            diff_rgb[o+1] = 0
                            diff_rgb[o+2] = 255
                        else:
                            diff_rgb[o] = ra // 3
                            diff_rgb[o+1] = ga // 3
                            diff_rgb[o+2] = ba // 3
                pixel_info["totalPixels"] = wa * ha_px
                pixel_info["differingPixels"] = diff_px
                if diff_px == 0:
                    png_identical += 1
                    pixel_info["pixelIdentical"] = True
                else:
                    png_diff += 1
                    pixel_info["pixelIdentical"] = False
                    pixel_info["bbox"] = [min_x, min_y, max_x, max_y]
                    diff_name = f"{cell}__{st}__diff.png"
                    open(os.path.join(diff_dir, diff_name), "wb").write(encode_png_rgb(wa, ha_px, diff_rgb))
                    pixel_info["diffImage"] = diff_name
                    print(f"NOTE PNG DIFF {cell} {st}: {diff_px}/{wa*ha_px} px bbox={pixel_info['bbox']} diff={diff_name}")

        rows.append({
            "cell": cell,
            "state": st,
            "htmlIdentical": html_eq,
            "refHtmlSha256": sa["htmlSha256"],
            "candHtmlSha256": sb["htmlSha256"],
            "png": pixel_info,
        })

report = {
    "refDir": dir_a,
    "candDir": dir_b,
    "refHead": ma.get("headCommit"),
    "candHead": mb.get("headCommit"),
    "htmlIdenticalCount": html_pass,
    "htmlDiffCount": html_fail,
    "pngIdenticalCount": png_identical,
    "pngDiffCount": png_diff,
    "states": rows,
}
with open(report_path, "w", encoding="utf-8") as f:
    json.dump(report, f, indent=2)
    f.write("\n")

if html_fail > 0:
    print(f"OVERALL PARITY: FAIL ({html_pass}/{html_pass+html_fail} outerHTML identical, {html_fail} differed; png identical={png_identical}, png diff={png_diff})")
    sys.exit(1)
print(f"OVERALL PARITY: PASS ({html_pass}/{html_pass+html_fail} outerHTML byte-identical; png identical={png_identical}, png diff={png_diff})")
PY
}

say "S0c parity driver: mode=${MODE} out=${OUT_DIR} ref=${REF_DIR:-<none>}"

if [[ "$MODE" == "reference" ]]; then
  capture_matrix "ref-run1" "$OUT_DIR"
  capture_matrix "ref-run2" "$scratch/ref-run2"
  compare_manifests "$OUT_DIR" "$scratch/ref-run2" "$OUT_DIR/self-determinism.json" "$OUT_DIR/self-diffs"
  say "REFERENCE CAPTURED & SELF-DETERMINISM VERIFIED: $OUT_DIR"
elif [[ "$MODE" == "compare" ]]; then
  capture_matrix "candidate" "$OUT_DIR"
  compare_manifests "$REF_DIR" "$OUT_DIR" "$OUT_DIR/parity-compare.json" "$OUT_DIR/diffs"
elif [[ "$MODE" == "selfcheck" ]]; then
  capture_matrix "runA" "$OUT_DIR/runA"
  capture_matrix "runB" "$OUT_DIR/runB"
  compare_manifests "$OUT_DIR/runA" "$OUT_DIR/runB" "$OUT_DIR/parity-selfcheck.json" "$OUT_DIR/diffs"
fi
