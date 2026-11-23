#!/bin/bash
source /mnt/c/Users/hp/AppData/Local/Temp/review-r3/env.sh
cd "$LAB" || exit 1
echo "=== A1 solution build" | tee "$EVID/A1-build-sln.log"
"$DOTNET" --version | tee -a "$EVID/A1-build-sln.log"
"$DOTNET" build MultiRackSitkaP1Lab.sln --disable-build-servers 2>&1 | tee -a "$EVID/A1-build-sln.log" | tail -8
echo "=== A2 Ui build" | tee "$EVID/A2-build-ui.log"
"$DOTNET" build src/Ui/SitkaP1.Ui.csproj --disable-build-servers 2>&1 | tee -a "$EVID/A2-build-ui.log" | tail -8
echo "=== A3 Core suite" | tee "$EVID/A3-core.log"
ls tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll
"$DOTNET" tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll 2>&1 | tee -a "$EVID/A3-core.log" | tail -12
echo "core exit: ${PIPESTATUS[0]}" | tee -a "$EVID/A3-core.log"
grep -E 'fingerprint|passed' "$EVID/A3-core.log" | sed -E 's/^ +//' | head -12
echo "=== DONE build-and-core"
