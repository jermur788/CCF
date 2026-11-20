#!/usr/bin/env bash
set -euo pipefail

# Run from the registered, leased implementation lane. See handoff for the verified
# Flatpak instance and receipts. The harness is copied in only for the run.
mode=${1:?compile, exact-shadow, feature, historical or reference}
instance=${2:?Flatpak instance}
out=$(realpath "${3:?existing evidence directory}")
test -d "$out"
evidence=/media/jer/Files/CCF/Verification/CCF-857-BlankStarter-20261120
case "$out" in "$evidence"|"$evidence"/*) ;; *) exit 2 ;; esac
editor=/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity
hub=/home/jer/.var/app/com.unity.UnityHub
base=(flatpak enter "$instance" /usr/bin/env PATH=/usr/bin:/bin HOME=/home/jer
      XDG_DATA_HOME="$hub/data" XDG_CACHE_HOME="$hub/cache"
      UNITY_DISABLE_LINUX_TOOLCHAIN_MIGRATOR=1 UNITY_DISABLE_LINUX_AUTO_TOOLCHAIN_INSTALLATION=1)

if [[ $mode == compile ]]; then
    exec "${base[@]}" XDG_CONFIG_HOME="$hub/config" "$editor" -batchmode -nographics -quit \
        -projectPath "$PWD" -logFile "$out/candidate-pinned-compile.log"
fi
if [[ $mode == reference ]]; then
    staged=Assets/ForestPrototype/ScenarioReferenceVerification.cs
    if [[ -e $staged || -e $staged.meta ]]; then exit 2; fi
    cp -- Tools/Verification/ScenarioReferenceVerification.cs "$staged"
    trap 'rm -f -- "$staged" "$staged.meta"' EXIT
    "${base[@]}" XDG_CONFIG_HOME="$out/reference-config" "$editor" -batchmode -nographics -quit \
        -projectPath "$PWD" -executeMethod ScenarioReferenceVerification.ReportFrozen -logFile "$out/reference-console.log"
    exit
fi
if [[ $mode != exact-shadow && $mode != feature && $mode != historical ]]; then exit 2; fi

source=Tools/Verification/BlankStarter/BlankStarterQualification.cs
staged=Assets/ForestPrototype/BlankStarterQualification.cs
if [[ -e $staged || -e $staged.meta ]]; then
    printf 'STOP: disposable harness path already in use\n' >&2
    exit 2
fi
cp -- "$source" "$staged"
trap 'rm -f -- "$staged" "$staged.meta"' EXIT

graphics=(DISPLAY=:0 XAUTHORITY=/home/jer/.Xauthority XDG_SESSION_TYPE=x11 SDL_VIDEODRIVER=x11
          CCF_DIAG_DIR="$out" CCF_BLANK_MODE="$mode")
args=("$editor" -projectPath "$PWD" -executeMethod BlankStarterQualification.Begin -logFile "$out/$mode-console.log")
if [[ $mode == exact-shadow || $mode == feature ]]; then
    config=/home/jer/.config
    if [[ $mode == feature ]]; then
        config="$out/feature-config"
        slot="$config/unity3d/DefaultCompany/CCF"
        launcher=()
    else
        slot="$out/exact-shadow-directory"
        # This launch cannot resolve the host CCF directory. The initial live attempt and its
        # side effects remain separate historical evidence, not overwritten by this retry.
        launcher=(/bin/sh -ec
                  'CCF857_PARENT_MOUNT_NS=$(readlink /proc/self/ns/mnt); export CCF857_PARENT_MOUNT_NS; exec /usr/bin/unshare -Ur -m /usr/bin/python3 "$@"'
                  sh "$PWD/Tools/Verification/BlankStarter/shadow-save-dir.py" "$slot")
    fi
    mkdir -p "$slot"
    printf '<?xml version="1.0" encoding="utf-8"?>\n<unity_prefs version_major="1" version_minor="1"><pref name="CCF.Learning.v1.rack.create" type="int">1</pref></unity_prefs>\n' > "$slot/prefs"
    fixture="${4:?preserved fixture}"
    cp -- "$fixture" "$slot/forest-save.json"
    save="$config/unity3d/DefaultCompany/CCF/forest-save.json"
    "${base[@]}" "${graphics[@]}" XDG_CONFIG_HOME="$config" CCF_857_WITNESS_PATH="${save%/*}" "${launcher[@]}" \
        /usr/bin/strace -f -qq -e signal=none \
        -e trace=%file,read,readv,pread64,preadv,preadv2,write,writev,pwrite64,pwritev,pwritev2,close,fstat,mmap \
        -P "$save" -P "$save.tmp" -P "$save.bak" \
        -o "$out/$mode-save-access.strace" "${args[@]}"
    test ! -s "$out/$mode-save-access.strace"
    cmp -- "$fixture" "$slot/forest-save.json"
    test ! -e "$slot/forest-save.json.tmp"
    test ! -e "$slot/forest-save.json.bak"
else
    # The positive deliberately accesses a separate ext4 copy, never the user's restored directory.
    sandbox="$out/historical-config"
    slot="$sandbox/unity3d/DefaultCompany/CCF"
    mkdir -p "$slot"
    fixture="${4:?historical fixture path}"
    cp -- "$fixture" "$slot/forest-save.json"
    sha256sum "$fixture" > "$out/historical-before.sha256"
    "${base[@]}" "${graphics[@]}" XDG_CONFIG_HOME="$sandbox" CCF_857_HISTORICAL_PATH="$slot" "${args[@]}"
    sha256sum --check "$out/historical-before.sha256"
    cmp -- "$fixture" "$slot/forest-save.json.bak"
    sha256sum "$slot/forest-save.json" "$slot/forest-save.json.bak" > "$out/historical-round-trip.sha256"
fi
