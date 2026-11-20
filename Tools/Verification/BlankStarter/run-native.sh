#!/usr/bin/env bash
set -euo pipefail

# Run from the registered, leased implementation lane. See handoff for the verified
# Flatpak instance and receipts. The harness is copied in only for the run.
mode=${1:?compile, exact-shadow, feature, historical, reference, reference-continuation or enlarged80}
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
if [[ $mode == reference || $mode == reference-continuation || $mode == enlarged80 ]]; then
    gate=ScenarioReferenceVerification
    source=Tools/Verification/$gate.cs
    entry=ReportFrozen
    marker='^REFERENCE_ARCHIVE_INTEGRITY_PASS'
    override=0
    quit=(-quit)
    if [[ $mode == reference-continuation ]]; then
        entry=Begin
        marker='^REFERENCE_FUTURE_V1_CONTRACT_PASS'
        quit=()
    elif [[ $mode == enlarged80 ]]; then
        gate=Enlarged80Verification
        source=Tools/Verification/StandGeometry/$gate.cs
        entry=Begin
        marker='^ENLARGED80_VERIFY_PASS'
        override=''
        quit=()
    fi
    staged=Assets/ForestPrototype/$gate.cs
    if [[ -e $staged || -e $staged.meta ]]; then exit 2; fi
    mkdir "$out/$mode-config"
    cp -- "$source" "$staged"
    trap 'rm -f -- "$staged" "$staged.meta"' EXIT
    "${base[@]}" XDG_CONFIG_HOME="$out/$mode-config" CCF_STAND_GEOMETRY="$override" CCF_TEST_DIRECT_ENTRY=1 \
        "$editor" -batchmode -nographics "${quit[@]}" \
        -projectPath "$PWD" -job-worker-count 2 -executeMethod "$gate.$entry" -logFile "$out/$mode-console.log"
    grep -E "$marker" "$out/$mode-console.log"
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
    mkdir -p "$(dirname "$slot")"
    mkdir "$slot"
    printf '<?xml version="1.0" encoding="utf-8"?>\n<unity_prefs version_major="1" version_minor="1"><pref name="CCF.Learning.v1.rack.create" type="int">1</pref></unity_prefs>\n' > "$slot/prefs"
    fixture="${4:?preserved fixture}"
    cp -p -- "$fixture" "$slot/forest-save.json"
    printf 'CCF #857 disposable existing temporary save witness\n' > "$slot/forest-save.json.tmp"
    printf 'CCF #857 disposable existing backup save witness (distinct payload)\n' > "$slot/forest-save.json.bak"
    chmod 0600 "$slot/forest-save.json.tmp"
    chmod 0640 "$slot/forest-save.json.bak"
    touch -m -d '2026-11-18 10:00:00 UTC' "$slot/forest-save.json.tmp"
    touch -m -d '2026-11-18 11:00:00 UTC' "$slot/forest-save.json.bak"
    witness=Tools/Verification/BlankStarter/save_witness.py
    python3 "$witness" "$out" --shadow-slots "$slot" before
    save="$config/unity3d/DefaultCompany/CCF/forest-save.json"
    trace=(/usr/bin/strace -f -qq -e signal=none
        -e trace=%file,read,readv,pread64,preadv,preadv2,write,writev,pwrite64,pwritev,pwritev2,close,fstat,mmap \
        -P "$save" -P "$save.tmp" -P "$save.bak")
    launch_rc=0
    "${base[@]}" "${graphics[@]}" XDG_CONFIG_HOME="$config" CCF_857_WITNESS_PATH="${save%/*}" "${launcher[@]}" \
        "${trace[@]}" -o "$out/$mode-save-access.strace" "${args[@]}" || launch_rc=$?
    python3 "$witness" "$out" --shadow-slots "$slot" after
    cmp -- "$out/shadow-slots-before.json" "$out/shadow-slots-after.json"
    test "$launch_rc" = 0
    grep -E '^CCF857 PASS total=[0-9]+ mode=blank failure= runtimeErrors=0$' "$out/$mode-console.log"
    test -f "$out/$mode-save-access.strace"
    test ! -s "$out/$mode-save-access.strace"
    # Positive control after the negative: real stat/open/read of each staged file
    # through the same mount and filters, with payloads omitted from the control log.
    "${base[@]}" "${launcher[@]}" "${trace[@]}" -s 0 -o "$out/$mode-positive-control.strace" \
        /bin/sh -ec 'for p do /usr/bin/stat "$p" >/dev/null; /usr/bin/cat "$p" >/dev/null; done' \
        sh "$save" "$save.tmp" "$save.bak"
    for p in "$save" "$save.tmp" "$save.bak"; do
        grep -F "\"$p\"" "$out/$mode-positive-control.strace" | grep -E 'open(at|at2)?\('
    done
else
    # The positive deliberately accesses a separate ext4 copy, never the user's restored directory.
    sandbox="$out/historical-config"
    slot="$sandbox/unity3d/DefaultCompany/CCF"
    mkdir -p "$slot"
    fixture="${4:?historical fixture path}"
    cp -- "$fixture" "$slot/forest-save.json"
    sha256sum "$fixture" > "$out/historical-before.sha256"
    "${base[@]}" "${graphics[@]}" XDG_CONFIG_HOME="$sandbox" CCF_857_HISTORICAL_PATH="$slot" \
        CCF_STAND_GEOMETRY=0 "${args[@]}"
    grep -E '^CCF857 PASS total=[0-9]+ mode=historical failure= runtimeErrors=0$' "$out/$mode-console.log"
    sha256sum --check "$out/historical-before.sha256"
    cmp -- "$fixture" "$slot/forest-save.json.bak"
    sha256sum "$slot/forest-save.json" "$slot/forest-save.json.bak" > "$out/historical-round-trip.sha256"
fi
