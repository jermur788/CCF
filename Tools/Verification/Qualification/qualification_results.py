"""Pure receipt/coverage evaluation shared by verification runners; never waives process exits."""
import csv
from pathlib import Path
import re

DENSITY_PHASES = "RATIOS,ROLLS,ANCHORS,SITKA100,SITKACOMP,PARITY,CURVE,FAIRNESS,FAIRNESSHIGH,CONTINUITY,FINALBROADLEAF,LATEMIX,SAVELOAD,PERF,LONGRUN".split(',')
PERF_LABELS = {"PERF_ANNUAL_SITKA_MODEL1", "PERF_ANNUAL_SITKA_MODEL2_LEGACY_ROUTE", "PERF_ANNUAL_MIXED_BELOW_ONSET", "PERF_ANNUAL_MIXED_DENSE", "PERF_ANNUAL_OAK_DENSE", "PERF_PURE_PLAN"}
DENSITY_SCENARIOS = ["A_OAK_DENSE", "B_BEECH_DENSE", "C_MIXED_DENSE", "D_SITKA_DENSE_MODEL2", "E_OAK_YOUNG", "F_BEECH_YOUNG", "G_MIXED_YOUNG", "C1_MIXED_DENSE_MODEL1", "G1_MIXED_YOUNG_MODEL1"]


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
                'error': None if complete else 'LONGRUN filter selected nothing or produced missing/duplicate/unexpected measurement rows'}
    except (OSError, ValueError, KeyError, csv.Error) as error:
        return {'rows': 0, 'expected_rows': 0, 'complete': False, 'error': str(error)}


def evaluate_receipt(code, marker, text, unexpected, min_hard_checks=None, env=None, measurement_evidence=None):
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
        'game_view_dimensions': re.findall(r'U4_NOTE GAME_VIEW actual=(\d+x\d+)', text),
    }
