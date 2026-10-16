"""Pure receipt/coverage evaluation shared by verification runners; never waives process exits."""
import csv
import math
from pathlib import Path
import re

DENSITY_PHASES = "RATIOS,ROLLS,ANCHORS,SITKA100,SITKACOMP,PARITY,CURVE,FAIRNESS,FAIRNESSHIGH,CONTINUITY,FINALBROADLEAF,LATEMIX,SAVELOAD,PERF,LONGRUN".split(',')
PERF_LABELS = {"PERF_ANNUAL_SITKA_MODEL1", "PERF_ANNUAL_SITKA_MODEL2_LEGACY_ROUTE", "PERF_ANNUAL_MIXED_BELOW_ONSET", "PERF_ANNUAL_MIXED_DENSE", "PERF_ANNUAL_OAK_DENSE", "PERF_PURE_PLAN"}
DENSITY_SCENARIOS = ["A_OAK_DENSE", "B_BEECH_DENSE", "C_MIXED_DENSE", "D_SITKA_DENSE_MODEL2", "E_OAK_YOUNG", "F_BEECH_YOUNG", "G_MIXED_YOUNG", "C1_MIXED_DENSE_MODEL1", "G1_MIXED_YOUNG_MODEL1"]

# Schema of the density harness's component-ba-<geo>.csv (verification-only per-component basal-area evidence; see
# DensityQualification.cs ComponentBaHeader). The order is exact: a file whose header differs is rejected.
COMPONENT_BA_COLUMNS = [
    "scenario", "geo", "year", "route", "livingBefore", "baPreM2", "densityDeaths", "sitkaComponentDeaths", "genericDeaths",
    "sitkaBaLossM2", "genericBaLossM2", "combinedBaLossM2", "sitkaBaFraction", "genericBaFraction", "combinedBaFraction",
    "deadBaM2", "deadBaFraction", "sitkaFired", "genericFired", "bothFired", "attributionExact",
]
COMPONENT_BA_FLOAT_FIELDS = ("baPreM2", "sitkaBaLossM2", "genericBaLossM2", "combinedBaLossM2", "sitkaBaFraction",
                             "genericBaFraction", "combinedBaFraction", "deadBaM2", "deadBaFraction")
COMPONENT_BA_COUNT_FIELDS = ("livingBefore", "densityDeaths", "sitkaComponentDeaths", "genericDeaths")
COMPONENT_BA_FLAG_FIELDS = ("sitkaFired", "genericFired", "bothFired", "attributionExact")

# The harness serialises every basal-area and fraction field to five decimals (DensityQualification F(..., "0.00000")),
# so a self-consistent row can still show a small representation-only difference between independently rounded fields. The
# reader derives its tolerances from that 5-decimal quantum instead of an unrealistically tight absolute epsilon: it accepts
# what serialisation alone can explain and still rejects a difference materially larger than the quantum.
COMPONENT_BA_QUANTUM = 1e-5
COMPONENT_BA_FLOAT_SLACK = 1e-9

# Explicit LONGRUN evidence-format contract. CURRENT is the default and demands component-ba evidence. PRE_COMPONENT_BA is
# the historical format produced before component-BA instrumentation: ordinary LONGRUN evidence is still required and must
# be complete/valid, but component-ba is not expected. Historical mode must NEVER be inferred (from a missing file, a date,
# a candidate SHA or a directory name); a caller has to ask for it explicitly.
EVIDENCE_FORMAT_CURRENT = "current"
EVIDENCE_FORMAT_PRE_COMPONENT_BA = "pre-component-ba"
EVIDENCE_FORMATS = (EVIDENCE_FORMAT_CURRENT, EVIDENCE_FORMAT_PRE_COMPONENT_BA)


def normalize_evidence_format(value):
    """Resolve the LONGRUN evidence format. Omission (None) means CURRENT; any explicit empty/unknown value is rejected."""
    if value is None:
        return EVIDENCE_FORMAT_CURRENT
    if value not in EVIDENCE_FORMATS:
        raise ValueError("Unknown evidence format %r; valid formats: %s" % (value, ", ".join(EVIDENCE_FORMATS)))
    return value


def component_ba_sum_tolerance():
    """Worst-case representation-only gap for combined - (sitka + generic): three independently rounded values."""
    return 1.5 * COMPONENT_BA_QUANTUM + COMPONENT_BA_FLOAT_SLACK


def component_ba_total_tolerance():
    """Representation-only gap for combined - reported dead total: two independently rounded values."""
    return COMPONENT_BA_QUANTUM + COMPONENT_BA_FLOAT_SLACK


def component_ba_fraction_tolerance(ba_pre):
    """Gap between a stored fraction and its BA numerator divided by the denominator: the fraction rounds (half a quantum)
    and the numerator rounds (half a quantum divided by the denominator)."""
    tolerance = 0.5 * COMPONENT_BA_QUANTUM + COMPONENT_BA_FLOAT_SLACK
    if ba_pre > 0:
        tolerance += 0.5 * COMPONENT_BA_QUANTUM / ba_pre
    return tolerance


def component_ba_row_problems(row):
    """Per-row invariants. component total == reported total, zero components reported as zero, flags self-consistent."""
    name = f"{row.get('scenario', '?')} Y{row.get('year', '?')}"
    problems = []
    values = {}
    for field in COMPONENT_BA_FLOAT_FIELDS:
        try:
            value = float(row[field])
        except (TypeError, ValueError):
            problems.append(f"{name}: {field} is not a number")
            value = None
        if value is not None and not math.isfinite(value):
            problems.append(f"{name}: {field} is not finite")
        values[field] = value
    for field in COMPONENT_BA_COUNT_FIELDS:
        try:
            int(row[field])
        except (TypeError, ValueError):
            problems.append(f"{name}: {field} is not an integer")
    flags = {}
    for field in COMPONENT_BA_FLAG_FIELDS:
        flags[field] = row[field] in ("0", "1")
        if not flags[field]:
            problems.append(f"{name}: {field} is not 0/1")
    if problems:
        return problems
    sitka_ba, generic_ba, combined_ba = values["sitkaBaLossM2"], values["genericBaLossM2"], values["combinedBaLossM2"]
    dead_ba, ba_pre = values["deadBaM2"], values["baPreM2"]
    # component total == reported total (quantisation-aware: the fields are independently rounded to five decimals)
    if abs(combined_ba - (sitka_ba + generic_ba)) > component_ba_sum_tolerance():
        problems.append(f"{name}: combinedBaLossM2 {combined_ba} != sitka+generic {sitka_ba + generic_ba}")
    if abs(combined_ba - dead_ba) > component_ba_total_tolerance():
        problems.append(f"{name}: component total {combined_ba} != reported deadBaM2 {dead_ba}")
    fraction_tolerance = component_ba_fraction_tolerance(ba_pre)
    if abs(values["combinedBaFraction"] - values["deadBaFraction"]) > component_ba_total_tolerance():
        problems.append(f"{name}: combinedBaFraction {values['combinedBaFraction']} != deadBaFraction {values['deadBaFraction']}")
    for label, fraction_field, ba_field in (("combined", "combinedBaFraction", "combinedBaLossM2"),
                                            ("sitka", "sitkaBaFraction", "sitkaBaLossM2"),
                                            ("generic", "genericBaFraction", "genericBaLossM2")):
        if ba_pre > 0 and abs(values[fraction_field] - values[ba_field] / ba_pre) > fraction_tolerance:
            problems.append(f"{name}: {fraction_field} does not equal {ba_field} / baPreM2 within the 5-decimal quantum")
    # zero components reported as zero, fired components as positive
    zero_tolerance = 0.5 * COMPONENT_BA_QUANTUM + COMPONENT_BA_FLOAT_SLACK
    for label, fired_field, ba_field, count_field in (("sitka", "sitkaFired", "sitkaBaLossM2", "sitkaComponentDeaths"),
                                                      ("generic", "genericFired", "genericBaLossM2", "genericDeaths")):
        fired = row[fired_field] == "1"
        if fired and (values[ba_field] <= 0.0 or int(row[count_field]) <= 0):
            problems.append(f"{name}: {label} fired but {ba_field}/{count_field} is not positive")
        if not fired and (abs(values[ba_field]) > zero_tolerance or int(row[count_field]) != 0):
            problems.append(f"{name}: {label} did not fire but {ba_field}/{count_field} is not zero")
    both = row["bothFired"] == "1"
    if both != (row["sitkaFired"] == "1" and row["genericFired"] == "1"):
        problems.append(f"{name}: bothFired is not sitkaFired AND genericFired")
    if int(row["densityDeaths"]) != int(row["sitkaComponentDeaths"]) + int(row["genericDeaths"]):
        problems.append(f"{name}: densityDeaths != sitkaComponentDeaths + genericDeaths")
    # The rebuilt component plan must match the controller's own counts and the actual dead set, or the row is unattributable.
    if row["attributionExact"] != "1":
        problems.append(f"{name}: attributionExact is 0 (the rebuilt component plan did not match the controller)")
    return problems


def read_component_ba_evidence(directory, env, text):
    """Validate the harness's per-component basal-area CSV for a LONGRUN run.

    Fails (complete=False with an error) on a missing file, a schema mismatch, missing/duplicate/partial scenario-year rows,
    a geometry mismatch, non-numeric or non-finite values, inconsistent flags, a component total that does not match the
    reported total, or a zero component reported as non-zero. Numeric consistency uses a tolerance derived from the
    harness's five-decimal serialisation (COMPONENT_BA_QUANTUM), so representation-only rounding is accepted while a
    materially larger discrepancy is rejected. No density threshold is applied.
    """
    # absent is True ONLY where the file genuinely does not exist (the explicit `not path.is_file()` branch below). Every
    # other outcome (success, schema/parse/encoding error, OSError, malformed row) is absent=False: existence, once
    # established, means the evidence must be validated and can never masquerade as historical absence.
    failure = {"rows": 0, "expected_rows": 0, "complete": False, "error": None, "absent": False, "both_fired_rows": 0,
               "sitka_fired_rows": 0, "generic_fired_rows": 0, "attribution_mismatch_rows": 0, "path": None}
    try:
        phases, selected, horizon, _ = density_request(env)
        if "LONGRUN" not in phases:
            return None
        match = re.search(r"^QUAL_START geometry=(L40|E80)\b", text, re.MULTILINE)
        geo = match.group(1) if match else {"0": "L40", "1": "E80"}.get(env.get("CCF_STAND_GEOMETRY"))
        if not geo:
            return dict(failure, error="component-ba geometry evidence is missing")
        path = Path(directory) / f"component-ba-{geo}.csv"
        expected_geo = {"0": "L40", "1": "E80"}.get(env.get("CCF_STAND_GEOMETRY"))
        if expected_geo and expected_geo != geo:
            return dict(failure, path=str(path), error="component-ba geometry does not match the requested geometry")
        if not path.is_file():
            # The ONLY absence signal. The historical pre-component-ba format legitimately has no such file, so the caller's
            # explicit evidence-format decides whether this is acceptable. Absence is never inferred from an exception.
            return dict(failure, path=str(path), absent=True, error="component-ba file is missing: " + str(path))
        with path.open(newline="") as handle:
            reader = csv.DictReader(handle)
            if reader.fieldnames != COMPONENT_BA_COLUMNS:
                return dict(failure, path=str(path), error=f"component-ba schema mismatch: {reader.fieldnames}")
            rows = list(reader)
        expected = {(scenario, year) for scenario in selected for year in range(1, horizon + 1)}
        keys = [(row["scenario"], int(row["year"])) for row in rows]
        problems = []
        if not rows:
            problems.append("component-ba has no rows")
        if len(keys) != len(set(keys)):
            problems.append("component-ba has duplicate scenario-year rows")
        if set(keys) != expected:
            problems.append("component-ba is missing or has extra scenario-year rows")
        if len(rows) != len(expected):
            problems.append(f"component-ba row count {len(rows)} != expected {len(expected)}")
        for row in rows:
            if row["geo"] != geo:
                problems.append("component-ba row geometry does not match the run")
                break
        for row in rows[:1000]:
            problems.extend(component_ba_row_problems(row))
            if len(problems) > 20:
                break
        return {
            "path": str(path), "absent": False, "rows": len(rows), "expected_rows": len(expected), "complete": not problems,
            "error": None if not problems else "; ".join(problems[:20]),
            "both_fired_rows": sum(1 for row in rows if row["bothFired"] == "1"),
            "sitka_fired_rows": sum(1 for row in rows if row["sitkaFired"] == "1"),
            "generic_fired_rows": sum(1 for row in rows if row["genericFired"] == "1"),
            "attribution_mismatch_rows": sum(1 for row in rows if row["attributionExact"] != "1"),
        }
    except (OSError, ValueError, TypeError, KeyError, csv.Error) as error:
        return dict(failure, error=str(error))


def density_request(env):
    """Validate every supplied token, including otherwise-ignored mixed valid/invalid requests."""
    phases = {p.strip().upper() for p in env.get('CCF_QUAL_PHASES', ','.join(DENSITY_PHASES)).split(',')}
    unknown = phases - set(DENSITY_PHASES)
    if unknown:
        raise ValueError('Unknown CCF_QUAL_PHASES token(s): ' + repr(sorted(unknown)))
    available = {name.split('_')[0] for name in DENSITY_SCENARIOS}
    tokens = {p.strip() for p in env.get('CCF_QUAL_SCENARIOS', ','.join(sorted(available))).split(',')}
    unknown = tokens - available
    if unknown:
        raise ValueError('CCF_QUAL_SCENARIOS filter selected nothing for token(s): ' + repr(sorted(unknown)) + '; use short selectors such as B,F')
    selected = [name for name in DENSITY_SCENARIOS if name.split('_')[0] in tokens]
    try:
        horizon = int(env.get('CCF_QUAL_YEARS', '100'))
    except ValueError:
        horizon = 100
    horizon = max(100, horizon)
    measurement_only = phases <= {'LONGRUN', 'PERF'} and ('LONGRUN' not in phases or env.get('CCF_QUAL_REPLAY') == '0')
    return phases, selected, horizon, measurement_only


def read_longrun_evidence(directory, env, text):
    """Check the actual CSV's selected scenario/year keys, geometry and row count; no density thresholds."""
    try:
        phases, selected, horizon, _ = density_request(env)
        if 'LONGRUN' not in phases:
            return None
        match = re.search(r'^QUAL_START geometry=(L40|E80)\b', text, re.MULTILINE)
        geo = match.group(1) if match else {'0': 'L40', '1': 'E80'}.get(env.get('CCF_STAND_GEOMETRY'))
        if not geo:
            raise ValueError('LONGRUN geometry evidence is missing')
        expected_geo = {'0': 'L40', '1': 'E80'}.get(env.get('CCF_STAND_GEOMETRY'))
        if expected_geo and expected_geo != geo:
            raise ValueError('LONGRUN geometry does not match the requested geometry')
        path = Path(directory) / f'longrun-{geo}.csv'
        with path.open(newline='') as handle:
            rows = list(csv.DictReader(handle))
        keys = [(row['scenario'], int(row['year'])) for row in rows]
        expected = {(scenario, year) for scenario in selected for year in range(1, horizon + 1)}
        complete = bool(rows) and len(rows) == len(expected) and set(keys) == expected and all(row['geo'] == geo for row in rows)
        return {'path': str(path), 'rows': len(rows), 'expected_rows': len(expected), 'complete': complete,
                'error': None if complete else 'LONGRUN filter selected nothing or produced missing/duplicate/unexpected measurement rows',
                'component_ba': read_component_ba_evidence(directory, env, text)}
    except (OSError, ValueError, TypeError, KeyError, csv.Error) as error:
        return {'rows': 0, 'expected_rows': 0, 'complete': False, 'error': str(error),
                'component_ba': read_component_ba_evidence(directory, env, text)}


def evaluate_receipt(code, marker, text, unexpected, min_hard_checks=None, env=None, measurement_evidence=None,
                     evidence_format=None):
    lines = text.splitlines()
    compile_errors = [line for line in lines if "error CS" in line]
    failure_prefixes = (
        "UXQ_FAIL ", "UX_QUAL_FAILED ", "QUAL_FAIL ", "DENSITY_QUAL_FAIL ",
        "U4_FAIL ", "U4_GUIDANCE_FAIL ", "U4_GUIDANCE_ABORT ",
        "SCENARIO_HISTORY_FAIL ", "SCENARIO_HISTORY_VERIFY_FAIL ",
        "MIXED_DENSITY_FAIL ", "MIXED_DENSITY_VERIFY_FAIL ",
    )
    assertion_failures = [line for line in lines if line.startswith(failure_prefixes)]
    unverified = [line for line in lines if line.startswith("UXQ_UNVERIFIED ")]
    try:
        evidence_format = normalize_evidence_format(evidence_format)
    except ValueError as error:
        # An explicitly supplied, unknown/empty format rejects the invocation; it never silently falls back to historical.
        evidence_format = EVIDENCE_FORMAT_CURRENT
        assertion_failures.append(str(error))
    verdicts = list(re.finditer(r"^" + re.escape(marker) + r"(?:[ \t][^\n]*)?$", text, re.MULTILINE))
    verdict = verdicts[-1] if verdicts else None
    minimum = min_hard_checks
    coverage_errors = []
    measurement_only = False
    perf_measurements = []
    if marker == 'DENSITY_QUAL_PASS':
        try:
            phases, _, _, measurement_only = density_request(env) if env is not None else (set(), [], 100, False)
            if 'PERF' in phases:
                perf_measurements = re.findall(r'^QUAL_MEASURE (?:L40|E80) (PERF_[A-Z0-9_]+) ', text, re.MULTILINE)
                if len(perf_measurements) != len(PERF_LABELS) or set(perf_measurements) != PERF_LABELS:
                    coverage_errors.append('PERF expected six populated measurement records; phase coverage is incomplete.')
            if 'LONGRUN' in phases:
                if not measurement_evidence or not measurement_evidence.get('complete') or measurement_evidence.get('expected_rows', 0) <= 0 or measurement_evidence.get('rows') != measurement_evidence.get('expected_rows'):
                    coverage_errors.append('LONGRUN filter selected nothing or lacks complete expected measurement rows; downstream analysis cannot qualify this run.')
                component_ba = measurement_evidence.get('component_ba') if measurement_evidence else None
                component_ba_inconsistent = (not component_ba or not component_ba.get('complete')
                                             or component_ba.get('rows') != component_ba.get('expected_rows'))
                detail = (component_ba or {}).get('error') or 'component-ba rows are missing'
                if evidence_format == EVIDENCE_FORMAT_CURRENT:
                    if component_ba_inconsistent:
                        coverage_errors.append('component-ba per-component basal-area rows are missing or inconsistent; downstream analysis cannot split the mixed-stand basal-area loss (' + str(detail) + ').')
                elif not (component_ba and component_ba.get('absent') is True):
                    # Historical mode tolerates ONLY explicit physical absence. A file that exists, or whose absence could
                    # not be established, is validated normally; a present-but-invalid file fails.
                    if component_ba_inconsistent:
                        coverage_errors.append('component-ba was present but is incomplete or inconsistent; the pre-component-ba format tolerates only a genuinely absent file (' + str(detail) + ').')
        except ValueError as error:
            coverage_errors.append(str(error))
        if not measurement_only:
            minimum = max(1, minimum or 0)
    count = None
    if verdict:
        match = re.search(r"\b(?:hardChecks|checks|assertions)=(\d+)\b", verdict.group())
        if match:
            count = int(match.group(1))
    if minimum is not None and (count is None or count < minimum):
        coverage_errors.append(f"Hard-check coverage missing: expected >= {minimum}, observed {count}; the phase/scenario filter may have selected nothing. Qualification cannot PASS.")
    assertion_failures.extend(coverage_errors)
    gate_ok = verdict is not None and not compile_errors and not assertion_failures
    tail = text[verdict.end():] if verdict else ''
    shutdown_path = 'SIGSEGV' in tail and ('ShutdownControllerHIDAPI' in tail or 'Application::CoreShutdown' in tail)
    only_crash_artifacts = all(re.fullmatch(r'\?\? mono_crash\.[^/\\\s]+', item) for item in unexpected)
    shutdown_crash = gate_ok and shutdown_path and only_crash_artifacts and code in (255, 139, -11)
    passed = gate_ok and code == 0 and not unexpected and not shutdown_path
    status = ('PASS_WITH_UNVERIFIED' if unverified else 'PASS') if passed else 'FAIL'
    if shutdown_crash:
        process_outcome = 'VERDICT_PASS_POST_SHUTDOWN_CRASH'
    elif gate_ok and code == 0 and not shutdown_path:
        process_outcome = 'PASS_CLEAN_SHUTDOWN'
    else:
        process_outcome = 'PROCESS_OR_GATE_FAILURE'
    return {
        'status': status, 'exit_code': code, 'marker_found': verdict is not None,
        'gate_verdict': ('PASS_WITH_UNVERIFIED' if unverified else 'PASS') if gate_ok else 'FAIL',
        'process_outcome': process_outcome, 'compile_errors': compile_errors,
        'assertion_failures': assertion_failures, 'unverified_checks': unverified,
        'hard_checks': count, 'min_hard_checks': minimum, 'coverage_errors': coverage_errors,
        'measurement_only': measurement_only, 'measurement_evidence': measurement_evidence, 'perf_measurements': perf_measurements,
        'measurement_verdict': 'PENDING_DOWNSTREAM_ANALYSIS' if measurement_only else None,
        'evidence_format': evidence_format,
        'game_view_dimensions': re.findall(r'U4_NOTE GAME_VIEW actual=(\d+x\d+)', text),
    }
