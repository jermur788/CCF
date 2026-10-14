#!/usr/bin/env python3
# Run actual fixture controls in .NET doubles; no Unity equivalence claim.
from pathlib import Path
import argparse,subprocess
ROOT=Path(__file__).resolve().parents[4]
def method(s,signature):
 start=s.index(signature);brace=s.index('{',start);end=brace+1;depth=1
 while depth:
  depth+=(s[end]=='{')-(s[end]=='}');end+=1
 return s[start:end]
def main():
 parser=argparse.ArgumentParser();parser.add_argument('--out',type=Path,required=True);args=parser.parse_args();args.out.mkdir(parents=True,exist_ok=True)
 broad=(ROOT/'Tools/Verification/ThreeSpecies/BroadleafAdultGrowthVerification.cs').read_text()
 mixed=(ROOT/'Tools/Verification/ThreeSpecies/MixedDensityVerification.cs').read_text()
 assert broad.count('e.AdvanceOneYear();')==1
 assert 'fixtureIds = new HashSet<string>(Enumerable.Range(0, 9)' in broad
 assert 'crowdedCentre.Diameter < isolatedD - 0.05f' in broad and 'Mathf.Abs(crowdedCentre.Height - isolatedH) < 1e-3f' in broad
 assert 'Living().Length == 9' in broad and 'lawOk' in broad
 assert 'm1["neutral"] == m2["neutral"] && m1["normal"] == m2["normal"]' in mixed
 assert 'known && m2["neutral"] == expectNeutral && m2["normal"] == expectNormal' in mixed
 assert 'Check(!v16Accepted' in mixed and 'e.GrowthModelVersion == sessionGrowth' in mixed
 constants=mixed[mixed.index('    private const string Growth1Neutral ='):mixed.index('    // Stand-geometry expectations')]
 selector=method(mixed,'private static bool TryLifecycleExpectation(')
 step=method(broad,'private void Step()')
 source='''using System;using System.Collections.Generic;using System.Linq;
class StandGeometryModel{public const int Legacy40=0,Enlarged80=1;}
enum FindObjectsInactive{Include} enum FindObjectsSortMode{None}
class ForestTree{public string TreeId;public ForestTree gameObject=>this;}
class Ecology{public int calls;public void AdvanceOneYear(){calls++;Program.trees.Add(new ForestTree{TreeId="recruit"+calls});}}
class Program{
 public static List<ForestTree> trees=new List<ForestTree>(); Ecology e=new Ecology();HashSet<string> fixtureIds=new HashSet<string>();int recruitsRemoved;
 static ForestTree[] FindObjectsByType<T>(FindObjectsInactive i,FindObjectsSortMode s)=>trees.ToArray();
 static void DestroyImmediate(ForestTree t)=>trees.Remove(t);
 static int checks;static void Check(bool b,string s){if(!b)throw new Exception(s);checks++;Console.WriteLine("PASS "+s);}
'''+constants+selector+step+'''
 static void Main(){
 Check(TryLifecycleExpectation(0,out var n,out var h,out var b)&&n=="7E57B9DAEF5BF4D0"&&h=="35E2BF1C2F55C2DE"&&b.Contains("accepted Legacy40"),"Legacy40 keeps accepted anchors");
 Check(TryLifecycleExpectation(1,out n,out h,out b)&&n=="1FE6639780636394"&&h=="702DE05F33384139"&&b.Contains("not accepted anchors"),"Enlarged80 separate observed expectation");
 Check(!TryLifecycleExpectation(99,out n,out h,out b)&&n==null&&h==null,"unknown geometry no fallback");
 foreach(var prefix in new[]{"O","B","S"}){
 trees.Clear();var p=new Program();p.fixtureIds=new HashSet<string>(Enumerable.Range(0,9).Select(k=>prefix+k.ToString("00")));foreach(var id in p.fixtureIds)trees.Add(new ForestTree{TreeId=id});var original=trees.ToArray();
 for(int y=0;y<60;y++)p.Step();
 Check(trees.Count==9&&trees.SequenceEqual(original),prefix+" original adult identities and order preserved");
 Check(p.recruitsRemoved==60&&p.e.calls==60,prefix+" one actual annual step, injected recruits removed each year");
 }
 Console.WriteLine("FIXTURE_CONTROLS_PASS checks="+checks+" static-contracts=7 NOT_UNITY_VERIFIED");
 }
}'''
 (args.out/'Program.cs').write_text(source)
 (args.out/'check.csproj').write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net8.0</TargetFramework><OutputType>Exe</OutputType></PropertyGroup></Project>')
 return subprocess.run(['dotnet','run','--project',str(args.out/'check.csproj'),'-v','q']).returncode
if __name__=='__main__':raise SystemExit(main())
