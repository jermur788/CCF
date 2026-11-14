#!/usr/bin/env python3
"""Negative controls for the P0 test suites (M01-M17 original; M18-M30 added by MULTI-004 for the file-safety and layered-guard tests;
M31-M35 added by MULTI-006 for handle-anchored saves (R-1) and the read-loop size guard (R-2); M28 re-expressed for the new save code).

Copies the lab tree to a scratch directory (the real tree is never modified), breaks one safeguard at a time
and checks that the matching suite goes RED. A suite that stays green on a broken safeguard would be vacuous.

    python3 -I -B tests/run-mutation-controls.py <scratch-dir> [<results.json>]
"""
import json, os, shutil, subprocess, sys

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.dirname(HERE)

# (id, suite, file, old, new, what is being broken)
MUTATIONS = [
    ("M01", "core", "src/Core/Engine.cs", "if (t.Status != TreeStatus.Live) { notLive.Add(id); continue; }", "if (false) { notLive.Add(id); continue; }", "execute no longer rejects a dead tree in the plan"),
    ("M02", "core", "src/Core/Metrics.cs", "double r = dbhCm / 200.0;", "double r = dbhCm / 100.0;", "basal-area formula wrong by a factor of 4"),
    ("M03", "core", "src/Core/Geometry.cs", "if (OnSegment(poly[i], poly[(i + 1) % n], p)) return true;", "if (false) return true;", "boundary-on-edge stems no longer count as inside"),
    ("M04", "core", "src/Core/Engine.cs", "t.Status = TreeStatus.Felled;\n                    t.Mark = MarkState.Unmarked;", "if (id != plan.TreeIds[0]) t.Status = TreeStatus.Felled;\n                    t.Mark = MarkState.Unmarked;", "execution silently skips one planned tree (partial felling)"),
    ("M05", "core", "src/Core/LabSession.cs", "return err ?? StaleOpenApprovals(tx, \"ExperimentStepAdvanced\");", "return err;", "calendar step no longer stales approved/pending plans"),
    ("M06", "core", "src/Core/LabFile.cs", "        CheckHeader(bytes);\n        var root =", "        var root =", "header gate removed: wrong-format content reaches the body parser"),
    ("M07", "core", "src/Core/LabDataStore.cs", "if (!SaveFileName.IsMatch(fileName ?? \"\")) throw", "if (false) throw", "path-shape check removed from ReadSave"),
    ("M08", "core", "src/Core/Engine.cs", "s.ExperimentStep++; s.ForestRevision++; // calendar only: age, DBH, height untouched", "s.ExperimentStep++; s.ForestRevision++; foreach (var t in s.Trees) t.DbhMm += 1;", "calendar step quietly grows DBH"),
    ("M09", "core", "src/Core/SnapshotValidator.cs", "if (!Geometry.ContainsInclusive(s.Polygon, t.XMm, t.YMm)) throw", "if (false) throw", "out-of-polygon stems accepted"),
    ("M10", "core", "src/Core/LabSession.cs", "if (post != e.PostStateHash) err =", "if (false) err =", "replay stops verifying post-state hashes"),
    ("M11", "core", "src/Core/Engine.cs", "if (plan.BaseForestRevision != s.ForestRevision)\n            return new(\"RevisionMismatch\"", "if (false)\n            return new(\"RevisionMismatch\"", "execution ignores a forest-revision mismatch"),
    ("M12", "core", "src/Core/Engine.cs", "if (!string.Equals(plan.ApprovalSessionEpoch, guardEpoch, StringComparison.Ordinal))\n            return new(\"EpochMismatch\"", "if (false)\n            return new(\"EpochMismatch\"", "execution ignores a session-epoch mismatch"),
    ("M13", "core", "src/Core/Geometry.cs", "if (SegmentsIntersect(poly[i], poly[(i + 1) % n], poly[j], poly[(j + 1) % n]))", "if (false)", "bow-tie / self-intersecting rings accepted"),
    ("M14", "host", "src/Host/LabHost.cs", "if (!string.Equals(ctx.Request.Headers.Host.ToString(), expectedHost, StringComparison.OrdinalIgnoreCase))", "if (false)", "Host-header check removed"),
    ("M15", "host", "src/Host/LabHost.cs", "if (ctx.Request.Headers.TryGetValue(\"Origin\", out var origin) && !string.Equals(origin.ToString(), expectedOrigin, StringComparison.Ordinal))", "if (false)", "Origin check removed"),
    ("M16", "host", "src/Host/LabHost.cs", "k.Listen(IPAddress.Loopback, o.Port);", "k.ListenAnyIP(o.Port);", "listener bound to all interfaces instead of 127.0.0.1"),
    ("M17", "core", "src/Core/Engine.cs", "case EventType.PlanQueued:\n            {\n                if (plan.Stage != PlanStage.Approved)", "case EventType.PlanQueued:\n            {\n                if (false)", "queue accepted from any plan state"),
    # ---- MULTI-004 additions (M18-M30). M01-M17 above are the original P0 controls and are unchanged. --------------------------------
    # F-4: layered guards, each removed on its own (the reviewer's X01 / X06 / X16 / X22 / X08 shapes, expressed against the current code)
    ("M18", "core", "src/Core/Engine.cs", "if (plan.Stage is not (PlanStage.Approved or PlanStage.PendingWork)) return", "if (false) return", "PlanStale replay guard removed (forged STALE on a DRAFT/EXECUTED plan)"),
    ("M19", "core", "src/Core/Engine.cs", "if (t.Mark != MarkState.PlayerMarked || others.Contains(id)) unreserved.Add(id);", "if (t.Mark != MarkState.PlayerMarked) unreserved.Add(id);", "execute ignores a tree reserved by another open plan"),
    ("M20", "core", "src/Core/SnapshotValidator.cs", "if (reserved.TryGetValue(id, out var other)) throw", "if (false && reserved.TryGetValue(id, out var other)) throw", "validator allows a tree in two open plans"),
    ("M21", "core", "src/Core/LabDataStore.cs", "if (_io.Classify(RootPath, n) != LabEntryKind.Regular) continue;", "", "ListSaves no longer filters symlinks, FIFOs, folders and hard-linked files"),
    ("M22", "core", "src/Core/LabDataStore.cs", "switch (_io.Classify(RootPath, fileName!))", "switch (LabEntryKind.Regular)", "ReadSave per-entry pre-check removed (authoritative open alone remains)"),
    # F-1 / F-2: the authoritative open
    ("M23", "core", "src/Core/NoFollowFiles.cs", "int fd = OpenAt(dfd, name, O_RDONLY | ONoFollow | O_NONBLOCK | O_CLOEXEC);", "int fd = OpenAt(dfd, name, O_RDONLY | O_NONBLOCK | O_CLOEXEC);", "open follows symbolic links again (O_NOFOLLOW removed)"),
    ("M24", "core", "src/Core/NoFollowFiles.cs", "default: throw new LabException(\"NotRegularFile\", \"not a regular file\");", "default: break;", "open handle type is no longer checked (FIFO / folder accepted)"),
    ("M25", "core", "src/Core/NoFollowFiles.cs", "int fd = OpenAt(dfd, name, O_RDONLY | ONoFollow | O_NONBLOCK | O_CLOEXEC);", "int fd = OpenAt(dfd, name, O_RDONLY | ONoFollow | O_CLOEXEC);", "open may block on a FIFO again (O_NONBLOCK removed)"),
    ("M26", "core", "src/Core/NoFollowFiles.cs", "S_IFREG => nlink > 1 ? LabEntryKind.HardLinked : LabEntryKind.Regular,", "S_IFREG => LabEntryKind.Regular,", "hard-linked files accepted"),
    ("M27", "core", "src/Core/NoFollowFiles.cs", "int fd = OpenAt(AT_FDCWD, dir, O_RDONLY | ODirectory | ONoFollow | O_CLOEXEC);", "int fd = OpenAt(AT_FDCWD, dir, O_RDONLY | ODirectory | O_CLOEXEC);", "lab folder opened following links (a swapped-in folder link is accepted)"),
    # F-5: atomic save failure and error codes
    # M28 re-expressed by MULTI-006 against the handle-anchored save (same safeguard: a failed save removes its own temp file)
    ("M28", "core", "src/Core/NoFollowFiles.cs", "if (!renamed) UnlinkAt(dfd, tmp, 0);", "if (false) UnlinkAt(dfd, tmp, 0);", "failed atomic write leaves its temp file behind"),
    ("M29", "host", "src/Host/LabHost.cs", "code = CodeForStatus(code), message = why", "code = \"Forbidden\", message = why", "every refusal reports code 'Forbidden' again"),
    ("M30", "core", "src/Core/LabDataStore.cs", "default: throw new LabException(\"NotRegularFile\", \"a folder or special file already uses that name; refusing to replace it\");", "default: break;", "save onto a folder / special file no longer refused up front (surfaces as a raw write failure)"),
    # ---- MULTI-006 additions (M31-M35) ---------------------------------------------------------------------------------------------
    # R-1: each step of the save re-resolves the folder PATH instead of using the one trusted handle
    ("M31", "core", "src/Core/NoFollowFiles.cs", "int fd = OpenAtCreate(dfd, tmp, O_WRONLY", "int fd = OpenAtCreate(AT_FDCWD, Path.Combine(dir, tmp), O_WRONLY", "temp file created by folder path (not the trusted handle)"),
    ("M32", "core", "src/Core/NoFollowFiles.cs", "if (RenameAt(dfd, tmp, dfd, name) != 0)", "if (RenameAt(AT_FDCWD, Path.Combine(dir, tmp), AT_FDCWD, Path.Combine(dir, name)) != 0)", "rename by folder path (not the trusted handle)"),
    ("M33", "core", "src/Core/NoFollowFiles.cs", "if (!renamed) UnlinkAt(dfd, tmp, 0);", "if (!renamed) UnlinkAt(AT_FDCWD, Path.Combine(dir, tmp), 0);", "failure cleanup by folder path (orphans the temp in the trusted folder)"),
    ("M34", "core", "src/Core/LabDataStore.cs", "public void WriteAtomic(string dir, string fileName, byte[] bytes) => NoFollowFiles.WriteAtomic(dir, fileName, bytes, _writeStageHook);",
     "public void WriteAtomic(string dir, string fileName, byte[] bytes) { _writeStageHook?.Invoke(WriteStage.DirectoryOpened); string fin = Path.Combine(dir, fileName); string tmp = fin + \".tmp-\" + Guid.NewGuid().ToString(\"N\"); bool created = false; try { using (var fs = new FileStream(tmp, FileMode.CreateNew, FileAccess.Write, FileShare.None)) { created = true; fs.Write(bytes, 0, bytes.Length); } _writeStageHook?.Invoke(WriteStage.TempWritten); File.Move(tmp, fin, overwrite: true); } catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { if (created) { try { File.Delete(tmp); } catch (Exception) { } } throw new LabException(\"WriteFailed\", \"x\"); } }", "whole save reverted to the MULTI-004 path-based FileStream/File.Move/File.Delete (stage hooks kept so the swap tests still fire)"),
    # R-2: the read loop's second size check (the one that catches content beyond the statx size)
    ("M35", "core", "src/Core/NoFollowFiles.cs", "if (ms.Length > maxBytes) throw new LabException(\"FileTooLarge\", \"file exceeds the size limit\");", "", "read loop no longer re-checks the size (content larger than statx reported is returned)"),
]

def run(cmd, cwd):
    # Mutated code legitimately triggers "unreachable code" warnings; relax warnings-as-errors for the scratch copy only.
    cmd = cmd + ["-p:TreatWarningsAsErrors=false"]
    p = subprocess.run(cmd, cwd=cwd, capture_output=True, text=True, timeout=600)
    return p.returncode, p.stdout + p.stderr

def main():
    if len(sys.argv) < 2:
        print(__doc__); return 2
    scratch = os.path.abspath(sys.argv[1])
    out_json = sys.argv[2] if len(sys.argv) > 2 else None
    if os.path.exists(scratch): shutil.rmtree(scratch)
    shutil.copytree(ROOT, scratch, ignore=shutil.ignore_patterns("bin", "obj", ".lab-data", "evidence", "TestResults", "__pycache__"))
    suites = {"core": "tests/Core.Tests", "host": "tests/Host.Tests"}

    # control: the unmodified copy must be green, otherwise a RED result proves nothing
    for name, proj in suites.items():
        code, out = run(["dotnet", "run", "--project", proj], scratch)
        print(f"BASELINE {name}: exit {code}")
        if code != 0:
            print(out[-3000:]); return 1

    results = []
    only = set(filter(None, os.environ.get("MUT_ONLY", "").split(",")))
    for mid, suite, rel, old, new, what in MUTATIONS:
        if only and mid not in only: continue
        path = os.path.join(scratch, rel)
        original = open(path, encoding="utf-8").read()
        if original.count(old) != 1:
            results.append({"id": mid, "what": what, "verdict": "ERROR", "detail": f"pattern found {original.count(old)} times in {rel}"})
            print(f"{mid} ERROR pattern count {original.count(old)}"); continue
        try:
            open(path, "w", encoding="utf-8").write(original.replace(old, new))
            code, out = run(["dotnet", "run", "--project", suites[suite]], scratch)
            failed = [l.split(":")[0].replace("FAIL", "").strip() for l in out.splitlines() if l.startswith("FAIL ")]
            built = "error CS" not in out
            verdict = "CAUGHT" if (code != 0 and built and failed) else ("BUILD-BROKEN" if not built else "SURVIVED")
            results.append({"id": mid, "suite": suite, "file": rel, "what": what, "verdict": verdict, "failingTests": sorted(set(failed))[:12], "failingCount": len(set(failed))})
            print(f"{mid} {verdict:9} {len(set(failed)):2} failing — {what}")
        finally:
            open(path, "w", encoding="utf-8").write(original)
    orig = [r for r in results if r["id"] <= "M17"]          # the original P0 controls (M01-M17); M18-M30 MULTI-004, M31-M35 MULTI-006
    summary = {"baselineGreen": True, "mutations": len(results), "caught": sum(r["verdict"] == "CAUGHT" for r in results),
               "original17": {"run": len(orig), "caught": sum(r["verdict"] == "CAUGHT" for r in orig)}, "results": results}
    print(f"\nORIGINAL P0 CONTROLS (M01-M17): CAUGHT {summary['original17']['caught']} of {summary['original17']['run']}")
    print(f"ALL CONTROLS: CAUGHT {summary['caught']} of {summary['mutations']}")
    if out_json:
        json.dump(summary, open(out_json, "w"), indent=2)
    shutil.rmtree(scratch, ignore_errors=True)
    return 0 if summary["caught"] == summary["mutations"] else 1  # MUT_ONLY=M17,M03 reruns a subset

if __name__ == "__main__":
    sys.exit(main())
