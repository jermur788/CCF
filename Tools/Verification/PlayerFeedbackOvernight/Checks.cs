using System;
using System.Linq;
using System.Collections.Generic;
class Checks {
 static int count;
 static void Check(bool b,string s) { if(!b) throw new Exception(s); count++; }
 static void Main() {
  foreach(float angle in new[]{0f,0.6f,1.2f}) {
   float ax=(float)Math.Cos(angle),az=(float)Math.Sin(angle);
   var f=ScenarioOneRackBay.Frame.Anchored(new RackPoint(1,2),ax,az,9,new ScenarioOneRackBay.Corridor[0]);
   RackPoint At(float a,float b)=>new RackPoint(1+ax*a-az*b,2+az*a+ax*b);
   var m=new StandMapModel { HasProperty=true, SizeM=80, MinX=-40,MinZ=-40,TeachingFrame=f };
   var plan=StandMapDrawPlan.Build(m,600,MapDetail.Full);
   Check(plan.Count(p=>p.Symbol==MapSymbol.TeachingArea && p.Text==null)==3,"outline and two separators");
   Check(plan.Count(p=>p.Text!=null)==3,"third labels");
   foreach(int i in new[]{0,1,2}) { var p=At(-8+i*8,5); Check(f.Zone(p.X,p.Z)==i,"rotated zone"); }
   var outside=At(13,5);Check(!f.Contains(outside.X,outside.Z),"outside rejected");
   var trees=new List<ScenarioOneRackBay.Tree>();
   for(int i=0;i<3;i++){var p=At(-8+i*8,i==1?-5:5);trees.Add(new ScenarioOneRackBay.Tree("t"+i,p.X,p.Z,30,true,true));}
   var a=ScenarioOneRackBay.AnalyseTeachingRack(trees,new ScenarioOneRackBay.Order[0],f,3,5,2);
   Check(a.HasEnoughFuture,"three spread both sides");
   var q=At(-7,5);trees.Add(new ScenarioOneRackBay.Tree("extra",q.X,q.Z,30,true,true));
   a=ScenarioOneRackBay.AnalyseTeachingRack(trees,new ScenarioOneRackBay.Order[0],f,3,5,2);Check(!a.HasEnoughFuture && a.TooClose.Count>0,"fourth and close fail");
   trees.RemoveAt(3); trees[1]=new ScenarioOneRackBay.Tree("fell",At(0,-5).X,At(0,-5).Z,30,true,false,true);
   Check(!ScenarioOneRackBay.AnalyseTeachingRack(trees,new ScenarioOneRackBay.Order[0],f,3,5,2).HasEnoughFuture,"Fell never Crop");
   m.TeachingFrame=null;Check(!StandMapDrawPlan.Build(m,600,MapDetail.Full).Any(p=>p.Symbol==MapSymbol.TeachingArea),"no invented sample");
  }
  Console.WriteLine("Focused checks PASS "+count);
 }
}
