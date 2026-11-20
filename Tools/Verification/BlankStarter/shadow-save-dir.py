#!/usr/bin/env python3
"""Bind a disposable copy at the exact save-path name inside a private Linux namespace."""

import ctypes
import os
from pathlib import Path
import sys

TARGET = Path("/home/jer/.config/unity3d/DefaultCompany/CCF")
PREFIX = Path("/media/jer/Files/CCF/Verification/CCF-857-BlankStarter-20261120")


def main():
    shadow = Path(sys.argv[1]).resolve(strict=True)
    command = sys.argv[2:]
    if PREFIX not in shadow.parents or not command:
        sys.exit("STOP: witness shadow must be on ext4 under the registered evidence root.")
    # Never bind a mount in the original namespace, even if manually invoked.
    if os.environ.get("CCF857_PARENT_MOUNT_NS") in (None, os.readlink("/proc/self/ns/mnt")):
        sys.exit("STOP: fresh private mount namespace is required.")
    libc = ctypes.CDLL(None, use_errno=True)
    private, recursive, bind = 1 << 18, 1 << 14, 1 << 12
    if libc.mount(None, b"/", None, private | recursive, None) != 0:
        sys.exit("STOP: could not make mount propagation private: " + os.strerror(ctypes.get_errno()))
    if libc.mount(os.fsencode(shadow), os.fsencode(TARGET), None, bind, None) != 0:
        sys.exit("STOP: private bind failed: " + os.strerror(ctypes.get_errno()))
    if not os.path.samefile(shadow, TARGET) or not os.path.samefile(shadow / "forest-save.json", TARGET / "forest-save.json"):
        sys.exit("STOP: the Unity-visible directory is not the staged witness copy.")
    print("CCF857 private mount witness: " + str(shadow) + " visible as " + str(TARGET), flush=True)
    os.execv(command[0], command)


if __name__ == "__main__":
    main()
