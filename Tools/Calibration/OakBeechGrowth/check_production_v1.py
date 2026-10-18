#!/usr/bin/env python3
"""RETIRED / SUPERSEDED - do NOT use as a release gate.

This was the pre-density production calibration guard for the seven Oak/Beech
integrated-calibration source files. It is superseded by
Tools/Calibration/OakBeechGrowth/check_integrated_v1.py, which keeps the same chosen
values, the same [C] checks, the same kernel/source-expression guards and the same 360
frozen numerical expectations, and whose source-integrity guard is comment/whitespace
insensitive (Tools/Verification/source_normalization.py).

Why retired: this tool hashes the seven protected files by RAW bytes against a
pre-density baseline, so it false-fails current code (the assertion reports
Assets/ForestPrototype/ForestEcologyController.cs). It is excluded from the Tier-3
static list (Docs/Verification/ScenarioOneTier3Preflight/), which records it as
"retired; superseded by check_integrated_v1.py".

Replacement path: Tools/Calibration/OakBeechGrowth/check_integrated_v1.py
"""
import sys

_RETIRED_MESSAGE = (
    "PRODUCTION_CALIBRATION_V1_RETIRED: check_production_v1.py performs NO validation and must not be "
    "used as a gate. It hashes protected sources by raw bytes against a pre-density baseline and "
    "false-fails current code. Use Tools/Calibration/OakBeechGrowth/check_integrated_v1.py "
    "(INTEGRATED_CALIBRATION_V1_PASS, 360 checkpoints, normalized source guard). "
    "Non-zero exit here is a retirement marker, not a code failure."
)


def main():
    sys.stderr.write(_RETIRED_MESSAGE + "\n")
    print("PRODUCTION_CALIBRATION_V1_RETIRED superseded_by=Tools/Calibration/OakBeechGrowth/check_integrated_v1.py")
    return 3


if __name__ == '__main__':
    sys.exit(main())
