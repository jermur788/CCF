#!/usr/bin/env python3
"""MFM-06 mutation controls: proves the offline check actually fails when a production rule is broken.

Copies Assets/ForestPrototype/**/*.cs to a temporary tree OUTSIDE the repository, applies one deliberate defect at a time, runs
run_offline_check.py against that tree (--src-root) and requires it to FAIL. The unmutated copy must PASS first. Nothing in the
repository is modified. Exit 0 and MFM06_MUTATION_CONTROLS_PASS only if every mutant is killed.

Usage: run_mutation_controls.py [--out DIR]
"""
import argparse, os, shutil, subprocess, sys, tempfile

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(os.path.join(HERE, "..", "..", ".."))
FP = "Assets/ForestPrototype"

V = FP + "/ForestSaveValidation.cs"
A = FP + "/ScenarioOne/ScenarioReferenceArchive.cs"
MUTANTS = [
    ("M01 allow prescription id 0", V, "record.prescriptionId < 1", "false"),
    ("M02 allow duplicate prescription ids", V, "if (!byId.TryAdd(record.prescriptionId, record))", "byId[record.prescriptionId] = record; if (false)"),
    ("M03 allow allocator at or below an id in use", V, "record.prescriptionId >= scenario.nextPrescriptionId", "false"),
    ("M04 accept any rule version text", V, "if (!IsVersionText(record.ruleVersion))", "if (false)"),
    ("M05 accept a never-allocated source rack", V, "record.sourceRackId >= scenario.nextRackId", "false"),
    ("M06 accept an approval year after the save", V, "record.approvalYear > data.ecologicalYear)", "false)"),
    ("M07 accept an order naming an unknown prescription", V,
     'return $"work order {order.workOrderId} names unknown prescription {order.prescriptionId}";', "return null;"),
    ("M08 accept provenance on an ordinary order", V,
     'return HasPrescriptionLink(order) ? $"work order {order.workOrderId} has prescription provenance but no prescription" : null;', "return null;"),
    ("M09 accept prescription state in a pre-v23 save", V, 'return claimsState ? "a save older than version 23 cannot carry prescription state" : null;', "return null;"),
    ("M10 accept an order from an unapproved prescription", V, "if (owner.approvalState != PrescriptionApprovalState.Approved)", "if (false)"),
    ("M11 accept an order created before approval", V, "order.createdYear < owner.approvalYear", "false"),
    ("M12 do not require the allocator field in JSON", V, 'scenario["nextPrescriptionId"]?.Type != JTokenType.Integer ||', "false ||"),
    ("M13 accept event naming an unknown prescription", V, 'return $"management event {entry.eventId} names unknown prescription {entry.prescriptionId}";', "continue;"),
    ("M14 wrong schema number", FP + "/ForestSaveData.cs", "CurrentVersion = 23;", "CurrentVersion = 24;"),
    ("M15 order provenance default is not 'none'", FP + "/ScenarioOne/ScenarioOneWorkOrder.cs", 'public string prescriptionCoverageUnitId = "";', 'public string prescriptionCoverageUnitId = "x";'),
    ("M16 migration wipes v23 prescriptions too", FP + "/ScenarioOne/ScenarioOneSaveMigration.cs", "if (data == null || version >= 23) return;", "if (data == null) return;"),
    ("M17 hash no longer strips the default v23 run", A, '.Replace(",\\"nextPrescriptionId\\":1,\\"prescriptions\\":[]", "")', ""),
    ("M18 hash ignores prescription state", A, "        if (!PrescriptionStateIsDefault(data))\n            return json;\n", ""),
    ("M19 canonical ordering lost", A, "a.prescriptionId.CompareTo(b.prescriptionId)", "0"),
    ("M20 prescription validation not run by Validate", V, "return ValidatePrescriptions(data);", "return null;"),
    ("M21 disk read skips the prescription JSON check", FP + "/ForestSaveController.cs", "?? ForestSaveValidation.ValidatePrescriptionsJson(json, data)", ""),
    ("M22 restore would create a prescription", FP + "/ScenarioOne/ScenarioOneManager.Prescriptions.cs", "prescriptions = ClonePrescriptions(data.prescriptions);",
     "prescriptions = ClonePrescriptions(data.prescriptions); prescriptions.Add(new PrescriptionSaveData());"),
    ("M23 allocator allowed to move backwards", FP + "/ScenarioOne/ScenarioOneManager.Prescriptions.cs", "nextPrescriptionId = Mathf.Max(nextPrescriptionId, prescriptions.Max(item => item.prescriptionId) + 1);", "nextPrescriptionId = 1;"),
]


def run(src_root, out):
    result = subprocess.run([sys.executable, os.path.join(HERE, "run_offline_check.py"), "--src-root", src_root, "--out", out],
                            capture_output=True, text=True)
    return result.returncode, result.stdout + result.stderr


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--out", default=os.path.join(tempfile.gettempdir(), "ccf-mfm06-mutation"))
    args = parser.parse_args()
    shutil.rmtree(args.out, ignore_errors=True)
    tree = os.path.join(args.out, "tree")
    src = os.path.join(ROOT, FP)
    for directory, _, files in os.walk(src):
        for name in files:
            if name.endswith(".cs"):
                target = os.path.join(tree, os.path.relpath(os.path.join(directory, name), ROOT))
                os.makedirs(os.path.dirname(target), exist_ok=True)
                shutil.copy2(os.path.join(directory, name), target)
    code, text = run(tree, os.path.join(args.out, "run-clean"))
    if code != 0 or "MFM06_SAVE_OFFLINE_PASS" not in text:
        print(text[-3000:])
        print("CLEAN COPY DID NOT PASS - controls invalid")
        return 2
    print("clean copy: PASS")
    survivors = []
    for label, path, old, new in MUTANTS:
        target = os.path.join(tree, path)
        original = open(target).read()
        if original.count(old) != 1:
            print("MUTANT TEXT NOT UNIQUE/PRESENT: " + label + " (" + str(original.count(old)) + ")")
            return 2
        open(target, "w").write(original.replace(old, new))
        try:
            code, text = run(tree, os.path.join(args.out, "run-" + label.split()[0]))
        finally:
            open(target, "w").write(original)
        killed = code != 0 and "MFM06_SAVE_OFFLINE_PASS" not in text
        reason = next((l for l in text.splitlines() if "FAIL" in l or "error" in l.lower()), "(no message)")[:150]
        print(("KILLED   " if killed else "SURVIVED ") + label + " -> " + reason)
        if not killed:
            survivors.append(label)
    print("mutants=%d killed=%d survived=%d" % (len(MUTANTS), len(MUTANTS) - len(survivors), len(survivors)))
    if survivors:
        print("SURVIVORS: " + "; ".join(survivors))
        return 1
    print("MFM06_MUTATION_CONTROLS_PASS")
    return 0


if __name__ == "__main__":
    sys.exit(main())
