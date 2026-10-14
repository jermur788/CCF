from pathlib import Path
import subprocess,shutil,json
r=Path(__file__).resolve().parents[4];fp=r/'Assets/ForestPrototype';tool=r/'Tools/Verification/ThreeSpecies/OfflineLogic';root=Path('/tmp/ccf-n1-mutations');root.mkdir(exist_ok=True)
mutants={
 'discard-sitka-signal':('MixedDensityModel.Composite.cs','double reference = (double)referenceDeaths / stems.Count;','double reference = 0.0;'),
 'untapered-generic-pressure':('MixedDensityModel.Composite.cs','* (occupied > 0.0 ? nonReference / occupied : 0.0)','* 1.0'),
 'no-generic-mortality':('MixedDensityModel.Composite.cs','return CompositeMaximumGenericLoss * (1.0 - Math.Exp(-CompositeResponseStrength * excess * excess));','return 0.0;'),
 'species-biased-allocation':('MixedDensityModel.Composite.cs','priority[i] = weight > 0.0 ? -Math.Log(u) / weight : double.PositiveInfinity;','priority[i] = weight > 0.0 ? -Math.Log(u) / (weight * Weight(stems[i].Demand)) : double.PositiveInfinity;'),
 'broadleaf-in-sitka-rd':('SitkaSubsetDensitySignal.cs','if (stem.Demand == GrowingSpaceDemandClass.Reference) subset.Add(stem);','subset.Add(stem);')}
results=[]
for name,(file,a,b) in mutants.items():
 work=root/name;work.mkdir(exist_ok=True);files=[]
 for f in ['SitkaGrowthModel.cs','MixedDensityModel.cs','MixedDensityModel.Composite.cs','SitkaSubsetDensitySignal.cs']:
  s=(fp/f).read_text()
  if f==file:assert a in s;s=s.replace(a,b)
  (work/f).write_text(s);files.append(work/f)
 shutil.copyfile(tool/'CompositeDensityOfflineCheck.cs',work/'CompositeDensityOfflineCheck.cs');files.append(work/'CompositeDensityOfflineCheck.cs')
 items=''.join('<Compile Include="'+str(f)+'" />' for f in files)
 (work/'mutant.csproj').write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net8.0</TargetFramework><OutputType>Exe</OutputType><EnableDefaultCompileItems>false</EnableDefaultCompileItems></PropertyGroup><ItemGroup>'+items+'</ItemGroup></Project>')
 build=subprocess.run(['dotnet','build','-v','q','-c','Release'],cwd=work,capture_output=True,text=True);assert build.returncode==0,build.stdout
 run=subprocess.run(['dotnet','run','--no-build','-c','Release'],cwd=work,capture_output=True,text=True);(work/'result.txt').write_text(run.stdout+run.stderr)
 failures=[l for l in run.stdout.splitlines() if l.startswith('FAIL ')];assert run.returncode!=0 and failures,(name,'survived');results.append(dict(mutant=name,killed=True,failing_assertions=failures));print(name,'KILLED',len(failures),flush=True)
(root/'results.json').write_text(json.dumps(results,indent=2))
