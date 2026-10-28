#!/usr/bin/env python3
"""Execute pointer/keyboard requests with xdotool on a private display (RACK-V2-05 real-pointer gate).

The Unity harness (RackV2EditingVerification.cs) decides WHERE to aim: it measures how the OS pointer maps onto the game's
Mouse.current.position and writes request-NNNN.json files into the shared folder. This process only performs them with the real
desktop pointer and keyboard (xdotool), then writes done-NNNN. It makes no decisions and knows nothing about the game.

Request:  {"seq": 7, "ops": [{"op":"move","x":900,"y":540}, {"op":"down"}, {"op":"sleep","ms":40}, {"op":"up"}, ...]}
Ops:      move (absolute screen px) | down | up (left button) | sleep (ms) | key (tap) | keydown | keyup (X11 key names, e.g. Delete,
          BackSpace, shift) | sync (write sync-NNNN-name, wait for go-NNNN-name: the harness inspects the live state with the button still down)

usage: pointer_driver.py <shared folder> <DISPLAY>      stops when the harness writes <folder>/finished
"""
import json
import os
import subprocess
import sys
import time
from pathlib import Path

folder = Path(sys.argv[1])
display = sys.argv[2]
env = dict(os.environ, DISPLAY=display)
log = open(folder / "driver.log", "a", buffering=1)


def say(text):
    log.write("%.3f %s\n" % (time.time(), text))


def xdo(*args):
    say("xdotool " + " ".join(str(a) for a in args))
    subprocess.run(["xdotool", *[str(a) for a in args]], env=env, check=True, stdout=subprocess.DEVNULL, stderr=subprocess.STDOUT, timeout=20)


def perform(seq, op):
    kind = op.get("op")
    if kind == "move":
        xdo("mousemove", op["x"], op["y"])
    elif kind == "down":
        xdo("mousedown", 1)
    elif kind == "up":
        xdo("mouseup", 1)
    elif kind == "sleep":
        time.sleep(op.get("ms", 0) / 1000.0)
    elif kind == "key":
        xdo("key", op["key"])
    elif kind == "keydown":
        xdo("keydown", op["key"])
    elif kind == "keyup":
        xdo("keyup", op["key"])
    elif kind == "sync":
        name = op["name"]
        (folder / ("sync-%04d-%s" % (seq, name))).write_text("waiting")
        go = folder / ("go-%04d-%s" % (seq, name))
        deadline = time.monotonic() + 120
        while not go.exists():
            if time.monotonic() > deadline:
                raise RuntimeError("harness never released sync " + name)
            time.sleep(0.03)
    else:
        raise RuntimeError("unknown op " + str(kind))


handled = set()
say("driver started on " + display)
while not (folder / "finished").exists():
    for request in sorted(folder.glob("request-*.json")):
        seq = int(request.stem.split("-")[1])
        if seq in handled:
            continue
        try:
            data = json.loads(request.read_text())
            for op in data["ops"]:
                perform(seq, op)
            (folder / ("done-%04d" % seq)).write_text("done")
            handled.add(seq)
        except Exception as error:  # report and stop: a half-delivered gesture must not be repeated
            say("ERROR in request %d: %r" % (seq, error))
            (folder / "driver-error").write_text("request %d: %r\n" % (seq, error))
            subprocess.run(["xdotool", "mouseup", "1"], env=env, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
            sys.exit(1)
    time.sleep(0.04)
say("finished")
