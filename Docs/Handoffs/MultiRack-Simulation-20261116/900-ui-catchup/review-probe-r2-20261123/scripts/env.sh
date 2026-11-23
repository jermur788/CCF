#!/bin/bash
# Sourced by every review script. Pins the toolchain; never uses system dotnet 10.
export REVIEW_WT=/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage4OwnerUX-R2Review-20261123
export LAB="$REVIEW_WT/Tools/MultiRackSitkaP1Lab"
export DOTNET=/home/jer/.dotnet/dotnet
export DOTNET_ROOT=/home/jer/.dotnet
export PATH="/home/jer/.dotnet:$PATH"
export TMPDIR=/home/jer/.cache/review-r2-tmp
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
export EVID=/home/jer/.local/share/ccf-evidence/review-r2-owner-ux-20261123
export P0EXPECT=9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3
mkdir -p "$TMPDIR" "$EVID"
