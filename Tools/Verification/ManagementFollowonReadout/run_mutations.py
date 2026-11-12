#!/usr/bin/env python3
"""Bounded guard-removal checks. Mutants exist only in fresh external output directories."""
import argparse
import difflib
import pathlib
import shutil
import subprocess
from xml.sax.saxutils import escape


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--out', required=True, type=pathlib.Path)
    out = parser.parse_args().out.resolve()
    here = pathlib.Path(__file__).resolve().parent
    root = here.parents[2]
    if out == root or root in out.parents or out.exists():
        parser.error('--out must be a NEW directory outside this repository')
    exe = shutil.which('dotnet')
    if exe is None:
        parser.error('dotnet SDK required')
    out.mkdir(parents=True)
    source = root / 'Assets/ForestPrototype/ScenarioOne/ScenarioOneManagementFollowonReadout.cs'
    original = source.read_text()
    mutations = [
        ('global', '''        duplicateEvents.UnionWith((years ?? Array.Empty<YearHistoryView>()).Where(y => y != null)
            .SelectMany(y => y.Events).Where(e => e != null).GroupBy(e => e.eventId)
            .Where(g => g.Key <= 0 || g.Count() > 1).Select(g => g.Key));''', '',
         'FAILED global ID collision with OrderCreated'),
        ('target', '            && SameRecordedTarget(o, e)', '', 'FAILED retained order target mismatch')]
    for name, before, after, expected in mutations:
        assert original.count(before) == 1, 'guard changed; review mutation definition'
        work = out / name
        work.mkdir()
        mutated = original.replace(before, after, 1)
        mutant = work / source.name
        mutant.write_text(mutated)
        (work / 'mutation.diff').write_text(''.join(difflib.unified_diff(
            original.splitlines(True), mutated.splitlines(True), fromfile='qualified-helper', tofile=name, n=0)))
        sources = [mutant, here / 'OfflineContractTypes.cs', here / 'ManagementFollowonReadoutVerification.cs']
        items = '\n'.join('<Compile Include="' + escape(str(p), {'"': '&quot;'}) + '" />' for p in sources)
        project = work / 'Mutation.csproj'
        project.write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType>'
                           '<TargetFramework>net8.0</TargetFramework><LangVersion>9.0</LangVersion>'
                           '<EnableDefaultCompileItems>false</EnableDefaultCompileItems></PropertyGroup>'
                           '<ItemGroup>' + items + '</ItemGroup></Project>')
        build = subprocess.run([exe, 'build', str(project), '-c', 'Release', '-v', 'q'], capture_output=True, text=True)
        (work / 'build.log').write_text(f'exit={build.returncode}\n' + build.stdout + build.stderr)
        assert build.returncode == 0, name + ' mutation must compile before it counts'
        run = subprocess.run([exe, str(work / 'bin/Release/net8.0/Mutation.dll'), name], capture_output=True, text=True)
        (work / 'run.log').write_text(f'exit={run.returncode}\n' + run.stdout + run.stderr)
        assert run.returncode != 0 and expected in run.stderr, name + ' mutant survived or failed for wrong reason'
        print(name + ': KILLED; build exit=0, assertion process exit=' + str(run.returncode) + '; ' + expected)
    assert source.read_text() == original, 'production source must remain unchanged'
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
