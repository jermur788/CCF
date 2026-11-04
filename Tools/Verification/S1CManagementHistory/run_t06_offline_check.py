#!/usr/bin/env python3
"""Compile production history/text with the existing offline shim and test T-06 boundaries."""
from pathlib import Path
import run_t04_offline_check as runner
runner.SOURCES = [s for s in runner.SOURCES if not s.endswith('S1CProvenanceOfflineCheck.cs')]
runner.SOURCES.append(str(Path(__file__).with_name('S1CGuidanceOfflineCheck.cs')))
runner.PROJECT = runner.PROJECT.replace('S1CProvenanceOfflineCheck', 'S1CGuidanceOfflineCheck')
raise SystemExit(runner.main())
