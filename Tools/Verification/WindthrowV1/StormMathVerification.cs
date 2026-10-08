#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
public static class StormMathVerification
{
 struct Case { public string Id; public StormTreeContext Context; public Case(string id,StormTreeContext context){Id=id;Context=context;} }
 static int checks;
 static void Check(bool condition,string message){checks++;if(!condition)throw new Exception(message);}
 static string F(float value)=>value.ToString("R",CultureInfo.InvariantCulture);
 public static void Begin()
 {
  try{
   string dir=Environment.GetEnvironmentVariable("CCF_STORM_OUTPUT");
   Case[] cases={new Case("Y0_DOM",new StormTreeContext(14.8f,20.5f,14.6f,0.05f,0.0f)),
new Case("Y0_SUP",new StormTreeContext(9.5f,12.0f,14.6f,0.03f,0.0f)),
new Case("OLD_SLEND",new StormTreeContext(23.0f,27.0f,23.0f,0.04f,0.0f)),
new Case("OLD_SUPP",new StormTreeContext(17.0f,17.0f,23.0f,0.02f,0.0f)),
new Case("HEAVY_NEW",new StormTreeContext(23.0f,27.0f,23.0f,0.35f,2.0f)),
new Case("LIGHT_NEW",new StormTreeContext(23.0f,27.0f,23.0f,0.1f,1.0f)),
new Case("GAP_NEW",new StormTreeContext(23.0f,28.0f,23.0f,0.4f,1.6f)),
new Case("GAP_OLD",new StormTreeContext(26.0f,34.0f,26.0f,0.4f,0.2f)),
new Case("CROP_REL",new StormTreeContext(23.0f,34.0f,23.0f,0.15f,0.2f)),
new Case("EDGE_EST",new StormTreeContext(22.0f,32.0f,23.0f,0.55f,0.0f)),
new Case("SHORT_STOUT",new StormTreeContext(10.0f,22.0f,23.0f,0.6f,0.0f)),
new Case("OLD_STABLE",new StormTreeContext(31.0f,62.0f,31.0f,0.08f,0.0f))};
   string[][] pairs={new[]{"HEAVY_NEW","OLD_SLEND"},
new[]{"LIGHT_NEW","OLD_SLEND"},
new[]{"HEAVY_NEW","LIGHT_NEW"},
new[]{"GAP_NEW","GAP_OLD"},
new[]{"OLD_SLEND","Y0_DOM"},
new[]{"OLD_SLEND","OLD_SUPP"},
new[]{"OLD_SLEND","CROP_REL"},
new[]{"OLD_SLEND","SHORT_STOUT"},
new[]{"EDGE_EST","SHORT_STOUT"},
new[]{"GAP_NEW","EDGE_EST"},
new[]{"Y0_DOM","Y0_SUP"}};
   using(var table=new StreamWriter(Path.Combine(dir,"vulnerability_cases.csv")))
   using(var ranks=new StreamWriter(Path.Combine(dir,"vulnerability_rankings.csv")))
   using(var curves=new StreamWriter(Path.Combine(dir,"probability_curves.csv")))
   {
    table.WriteLine("candidate,case,vulnerability");ranks.WriteLine("candidate,higher,lower,direction_holds");curves.WriteLine("transform,vulnerability,intensity,chance");
    foreach(StormVulnerabilityCandidate candidate in Enum.GetValues(typeof(StormVulnerabilityCandidate)))
    {
     var scores=cases.ToDictionary(c=>c.Id,c=>StormWindthrow.Vulnerability(c.Context,candidate));
     foreach(var c in cases){Check(StormCalibration.Finite(scores[c.Id])&&scores[c.Id]>=0,"Finite candidate index");table.WriteLine(candidate+","+c.Id+","+F(scores[c.Id]));}
     foreach(var pair in pairs){bool holds=scores[pair[0]]>scores[pair[1]];ranks.WriteLine(candidate+","+pair[0]+","+pair[1]+","+holds);if(candidate==StormVulnerabilityCandidate.ReducedProposal)Check(holds,"Reduced proposal ordering "+pair[0]+">"+pair[1]);}
    }
    var shortTree=new StormTreeContext(4,8,20,.3f,0);
    Check(StormWindthrow.Vulnerability(shortTree,StormVulnerabilityCandidate.FullProposal)==0,"Full candidate has categorical short-tree immunity");
    Check(StormWindthrow.Vulnerability(shortTree,StormVulnerabilityCandidate.ReducedProposal)>0,"Reduced candidate leaves short trees resistant but nonimmune");
    foreach(StormProbabilityTransform transform in Enum.GetValues(typeof(StormProbabilityTransform)))
    {
     foreach(float vulnerability in new[]{0f,.001f,.1f,1f,10f,100f,float.MaxValue})
     {
      float previous=-1;
      foreach(float intensity in new[]{.000001f,.02f,.06f,.18f,1f}){float chance=StormWindthrow.FailureChance(vulnerability,intensity,transform);Check(StormCalibration.Finite(chance)&&chance>=0&&chance<1&&chance>=previous,"Bounded monotone intensity curve");previous=chance;curves.WriteLine(transform+","+F(vulnerability)+","+F(intensity)+","+F(chance));}
     }
     float earlier=-1;foreach(float vulnerability in new[]{0f,.001f,.1f,1f,10f,100f,float.MaxValue}){float chance=StormWindthrow.FailureChance(vulnerability,.18f,transform);Check(chance>=earlier,"Monotone vulnerability curve");earlier=chance;}
    }
    foreach(float vulnerability in new[]{.1f,1f,10f,100f})Check(StormWindthrow.FailureChance(vulnerability,.18f,StormProbabilityTransform.BoundedRational)<=StormWindthrow.FailureChance(vulnerability,.18f,StormProbabilityTransform.Exponential),"Rational transform restrains equal-load tails");
    Check(StormWindthrow.Vulnerability(cases[0].Context,StormVulnerabilityCandidate.ReducedProposal)==StormWindthrow.Vulnerability(cases[0].Context,StormVulnerabilityCandidate.ReducedProposal,1),"Neutral explicit site factor");
   }
   bool rejected=false;try{StormWindthrow.FailureChance(1,.18f,(StormProbabilityTransform)128);}catch(ArgumentException){rejected=true;}Check(rejected,"Unknown transform rejected");
   rejected=false;try{StormWindthrow.Vulnerability(new StormTreeContext(14,20,20,.2f,0),(StormVulnerabilityCandidate)128);}catch(ArgumentException){rejected=true;}Check(rejected,"Unknown vulnerability candidate rejected");
   UnityEngine.Debug.Log("STORM_MATH_VERIFICATION_PASS checks="+checks);EditorApplication.Exit(0);
  }catch(Exception error){UnityEngine.Debug.LogError("STORM_MATH_VERIFICATION_FAIL "+error);EditorApplication.Exit(1);}
 }
}
#endif
