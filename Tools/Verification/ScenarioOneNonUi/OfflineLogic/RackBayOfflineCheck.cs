// Plain .NET checks of the actual production lesson rules. The CSV is a live-captured Legacy40 core at the scene's 2.0 m
// spacing (core-stand-live-2m.csv), not the old 1.9 m generator replica. It is calibration data, not live Unity qualification.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

public static class RackBayOfflineCheck
{
    private static int checks, failures;
    // The five live authored layout centres, captured from the Enlarged80 planted rows at the scene's serialized 2.0 m spacing
    // (SCENARIO-ONE-APPROACH-ANCHOR-DIAGNOSIS-V1, 2026-10-22). Row jitter moves the centres a few centimetres off their nominal
    // 16 m spacing; the scene-spacing check below ties this list back to the scene instead of trusting the old 1.9 m assumption.
    private static readonly float[] Lines = { -31.4092f, -16.0152f, 0.0147f, 16.0224f, 31.3419f };
    private static readonly string[] Future = { "P0714", "P1109", "P0706" };
    private static void Check(bool ok, string name, string detail = "") { checks++; Console.WriteLine((ok ? "PASS " : "FAIL ") + name + (detail.Length > 0 ? " [" + detail + "]" : "")); if (!ok) failures++; }
    private sealed class Stand
    {
        public List<ScenarioOneRackBay.Tree> Trees = new List<ScenarioOneRackBay.Tree>();
        public List<ScenarioOneRackBay.Order> Orders = new List<ScenarioOneRackBay.Order>();
        public ScenarioOneRackBay.Analysis Analyse() => ScenarioOneRackBay.Analyse(Lines, Trees, Orders);
        public ScenarioOneRackBay.Tree Get(string id) => Trees.First(t => t.Id == id);
        public void Crop(string id) { int i = Trees.FindIndex(t => t.Id == id); var t = Trees[i]; Trees[i] = new ScenarioOneRackBay.Tree(t.Id,t.X,t.Z,t.DbhCm,t.Living,true); }
        public void Mark(string id) { int i = Trees.FindIndex(t => t.Id == id); var t = Trees[i]; Trees[i] = new ScenarioOneRackBay.Tree(t.Id,t.X,t.Z,t.DbhCm,t.Living,t.Crop,true); }
        public void Plan(string id, ScenarioOneRackBay.Disposition d = ScenarioOneRackBay.Disposition.Sold) { Orders.RemoveAll(o => o.TargetId == id); Orders.Add(new ScenarioOneRackBay.Order(id,true,false,d,-1)); }
        public void Execute(int year)
        {
            foreach (var o in Orders.Where(o => o.Open).ToList())
            {
                Orders.Remove(o); Orders.Add(new ScenarioOneRackBay.Order(o.TargetId,false,true,o.Disposition,year));
                int i=Trees.FindIndex(t=>t.Id==o.TargetId); var t=Trees[i]; Trees[i]=new ScenarioOneRackBay.Tree(t.Id,t.X,t.Z,t.DbhCm,false,false);
            }
        }
        public Stand Clone() => new Stand { Trees = new List<ScenarioOneRackBay.Tree>(Trees), Orders = new List<ScenarioOneRackBay.Order>(Orders) };
    }
    private static float SceneSpacing(string root)
    {
        string scene = File.ReadAllText(Path.Combine(root, "Assets/Scenes/ForestTest.unity"));
        var match = System.Text.RegularExpressions.Regex.Match(scene, @"latticeSpacingMeters:\s*([0-9.]+)");
        if (!match.Success) throw new InvalidOperationException("ForestTest has no serialized latticeSpacingMeters");
        return float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
    }
    private static Stand Load(string root)
    {
        var s = new Stand();
        foreach (string line in File.ReadAllLines(Path.Combine(root,"Tools/Verification/ScenarioOneNonUi/OfflineLogic/core-stand-live-2m.csv")).Skip(1))
        {
            var c=line.Split(','); float x=float.Parse(c[1],CultureInfo.InvariantCulture),z=float.Parse(c[2],CultureInfo.InvariantCulture),d=float.Parse(c[3],CultureInfo.InvariantCulture);
            bool rack=Lines.Any(l=>Math.Abs(z-l)<=1.2f);
            s.Trees.Add(new ScenarioOneRackBay.Tree(c[0],x,z,d,!rack,false));
            if(rack)s.Orders.Add(new ScenarioOneRackBay.Order(c[0],false,true,ScenarioOneRackBay.Disposition.Sold,0));
        }
        return s;
    }
    private static float Dist(ScenarioOneRackBay.Tree a,ScenarioOneRackBay.Tree b) => (float)Math.Sqrt((a.X-b.X)*(a.X-b.X)+(a.Z-b.Z)*(a.Z-b.Z));
    private static void PlanInitial(Stand s)
    {
        for(int guard=0;guard<200;guard++)
        {
            var a=s.Analyse(); var weakest=a.Future.OrderBy(t=>t.ReleaseShare).First();
            if(weakest.ReleaseShare>=ScenarioOneRackBay.FirstReleaseShare)return;
            var target=s.Get(weakest.Id);
            var next=s.Trees.Where(t=>t.Living&&!t.Crop&&a.Contains(t.X,t.Z)&&Dist(t,target)<=8&&!s.Orders.Any(o=>o.TargetId==t.Id&&o.Open))
                .OrderByDescending(t=>ForestEcologyController.HegyiTerm(t.DbhCm,target.DbhCm,Dist(t,target))).First();
            s.Plan(next.Id);
        }
        throw new Exception("Initial release unreachable");
    }
    private static ScenarioOneTeachingSequence.Facts Facts(ScenarioOneRackBay.Analysis a, bool reviewed=false, bool completed=false) => new ScenarioOneTeachingSequence.Facts
    {
        Bay=a,AccessOpen=true,AccessReviewed=true,CropTrees=a.HasEnoughFuture?3:0,ReleasePlanned=a.ReleasePlanned,
        ReleaseExecuted=a.ReleaseExecuted,DeadwoodExecuted=a.MixedExecuted,ReleaseReviewed=reviewed,RackReviewComplete=completed
    };
    private static void ReviewAll(Stand s,ScenarioOneRackReview review,ref int serial)
    {
        var a=s.Analyse();
        foreach(var f in a.Future)
        {
            review.Observe(a,f.Id,f.X,f.Z,++serial);
            Check(review.Decide(a,f.Id,f.X,f.Z,ScenarioOneRackReview.Decision.RetainHere),"retain selected tree after genuine revisit "+f.Id);
        }
        for(int part=0;part<6;part++)
        {
            var t=s.Trees.First(t=>t.Living&&a.Portion(t.X,t.Z)==part);
            review.Observe(a,t.Id,t.X,t.Z,++serial);
            Check(review.Decide(a,t.Id,t.X,t.Z,ScenarioOneRackReview.Decision.RetainHere),"review retained stand portion "+part);
        }
    }
    public static int Run(string root)
    {
        string eco=File.ReadAllText(Path.Combine(root,"Assets/ForestPrototype/ForestEcologyController.cs"));
        float spacing=SceneSpacing(root);
        float layoutSpacing=Math.Min(Lines[2]-Lines[1],Lines[3]-Lines[2])/8f;
        Check(Math.Abs(layoutSpacing-spacing)<.1f,"captured rack layout follows the scene's serialized planting spacing",
            "scene "+spacing.ToString("0.###",CultureInfo.InvariantCulture)+" m, captured "+layoutSpacing.ToString("0.###",CultureInfo.InvariantCulture)+" m");
        Check(eco.Contains("public const float HegyiCutoffMeters = 8f;")&&eco.Contains("return (neighbourDbhCm / Mathf.Max(1f, targetDbhCm)) / Mathf.Max(0.5f, distanceMeters);"),"offline Hegyi stub matches production");
        Check(ScenarioOneRackBay.FutureTreeSpacingM==8f,"8 m model-linked spacing retained");
        var fresh=Load(root);var selected=fresh.Clone();foreach(string id in Future)selected.Crop(id);
        var a=selected.Analyse();
        Check(!ScenarioOneRackBay.Analyse(new float[0],fresh.Trees,fresh.Orders).Applies,"legacy no-rack fallback retained");
        float halfWidth=Math.Min(Lines[2]-Lines[1],Lines[3]-Lines[2])*.5f;
        Check(a.RackCount==5&&a.RackIndex==2&&Math.Abs(a.HalfWidthM-halfWidth)<.001,"middle rack footprint is half the gap to its neighbouring racks",
            "half width "+a.HalfWidthM.ToString("0.###",CultureInfo.InvariantCulture)+" m");
        int footprint=fresh.Trees.Count(t=>t.Living&&a.Contains(t.X,t.Z));
        double areaBasal=fresh.Trees.Where(t=>t.Living&&a.Contains(t.X,t.Z)).Sum(t=>Math.PI*Math.Pow(t.DbhCm/200.0,2.0));
        Check(footprint==69,"live 2.0 m fixture teaching footprint contains 69 trees");
        Check(Math.Abs(areaBasal-1.29490)<.00001,"live 2.0 m fixture footprint holds 1.29490 m2 of initial woodland BA",
            areaBasal.ToString("R",CultureInfo.InvariantCulture));
        Check(a.HasEnoughFuture&&a.NearSelected&&a.MiddleSelected&&a.FarSelected&&a.BothSides,"one selection per zone and both sides");
        Check(!a.ReleaseExecuted&&!a.ReleasePlanned,"selection is not release");
        Check(!a.Contains(0,0)&&!a.Contains(13,3)&&!a.Contains(0,a.CentreZ+a.HalfWidthM+0.05f),"corridor and footprint boundaries excluded");
        var clustered=fresh.Clone();foreach(string id in new[]{"P0616","P1112","P1415"})clustered.Crop(id);
        Check(!clustered.Analyse().HasEnoughFuture&&ScenarioOneTeachingSequence.Current(Facts(clustered.Analyse()))==1,"review counterexample clustered triple cannot finish Objective 2");
        var synthetic=new List<ScenarioOneRackBay.Tree>{new ScenarioOneRackBay.Tree("a",-9,3,10,true,true),new ScenarioOneRackBay.Tree("b",0,3,10,true,true),new ScenarioOneRackBay.Tree("c",9,3,10,true,true)};
        Check(!ScenarioOneRackBay.Analyse(Lines,synthetic,null).HasEnoughFuture,"all zones on only one side fail");
        synthetic[1]=new ScenarioOneRackBay.Tree("b",0,-3,2,true,true);
        Check(ScenarioOneRackBay.Analyse(Lines,synthetic,null).HasEnoughFuture,"player may retain a small promising tree; no unsupported DBH dominance test");
        synthetic[0]=new ScenarioOneRackBay.Tree("a",-5,3,10,true,true);
        Check(!ScenarioOneRackBay.Analyse(Lines,synthetic,null).HasEnoughFuture,"8 m spacing remains enforced across zones");
        synthetic[0]=new ScenarioOneRackBay.Tree("a",-9,3,10,true,true);
        // Rotate and translate the same points and rack together; rule results must be unchanged.
        foreach(float angle in new[]{35f,90f,145f})
        {
            var dir=RackGeometry.Direction(angle); Func<float,float,RackPoint> world=(x,z)=>new RackPoint(23+x*dir.X-z*dir.Z,-17+x*dir.Z+z*dir.X);
            var rotated=synthetic.Select(t=>{var p=world(t.X,t.Z);return new ScenarioOneRackBay.Tree(t.Id,p.X,p.Z,t.DbhCm,true,true);}).ToList();
            var frame=new ScenarioOneRackBay.Frame(world(-40,0),world(40,0),a.HalfWidthM);
            var r=ScenarioOneRackBay.Analyse(Lines,rotated,null,frame);
            Check(r.HasEnoughFuture&&r.BothSides&&rotated.Select(t=>r.Portion(t.X,t.Z)).SequenceEqual(synthetic.Select(t=>a.Portion(t.X,t.Z))),"zones and sides follow actual rack-relative frame at "+angle+" degrees");
        }
        var trivial=fresh.Clone();trivial.Crop(Future[0]);
        foreach(var t in trivial.Trees.Where(t=>t.Living&&!t.Crop&&a.Contains(t.X,t.Z)).Take(2))trivial.Plan(t.Id);
        trivial.Execute(2);
        Check(!trivial.Analyse().ReleaseExecuted&&ScenarioOneTeachingSequence.Current(Facts(trivial.Analyse(),true,true))==1,"old one-tree/two-removal route remains impossible");
        var outside=selected.Clone(); var target=outside.Get(Future[2]);
        var neighbour=outside.Trees.Where(t=>t.Living&&!a.Contains(t.X,t.Z)&&Dist(t,target)<=8).First();
        outside.Plan(neighbour.Id,ScenarioOneRackBay.Disposition.Deadwood);var op=outside.Analyse();outside.Execute(2);var od=outside.Analyse();
        Check(op.PlannedRemovals==0&&op.DeadwoodPlanned==0&&op.MinPlannedAndExecuted==0&&op.RemovedBasalShare==0,"outside-footprint planned competitors give no release/fate/cap credit");
        Check(od.ExecutedRemovals==0&&od.DeadwoodExecuted==0&&od.ReleaseYear==-1&&!od.ReleaseExecuted,"outside-footprint executed competitors cannot complete the lesson");
        var partial=selected.Clone();var inside=partial.Trees.First(t=>t.Living&&!t.Crop&&a.Contains(t.X,t.Z)&&Dist(t,partial.Get(Future[2]))<8);
        partial.Plan(inside.Id);float partialShare=partial.Analyse().Future.First(f=>f.Id==Future[2]).ReleaseShare;
        partial.Plan(neighbour.Id);partial.Execute(2);
        Check(Math.Abs(partial.Analyse().Future.First(f=>f.Id==Future[2]).ExecutedShare-partialShare)<1e-6f,"outside felling cannot inflate credited inside share by dropping out of the denominator");
        var unrelated=selected.Clone();var away=unrelated.Trees.First(t=>t.Living&&a.Contains(t.X,t.Z)&&Future.All(id=>Dist(t,unrelated.Get(id))>8));
        unrelated.Plan(away.Id);var ua=unrelated.Analyse();
        Check(ua.MinPlannedAndExecuted==0&&ua.PlannedRemovals==0&&ua.RemovedBasalShare>0,"non-competitor inside footprint still enters complete safeguard");
        var beforeReview=new ScenarioOneRackReview();beforeReview.Synchronize(a,true,0);var pre=a.Future[0];beforeReview.Observe(a,pre.Id,pre.X,pre.Z,1);
        Check(!beforeReview.Decide(a,pre.Id,pre.X,pre.Z,ScenarioOneRackReview.Decision.RetainHere),"inspection before executed initial intervention earns no later-review decision");
        var release=selected.Clone();PlanInitial(release);a=release.Analyse();
        Console.WriteLine("CALIBRATION footprint="+footprint+" trees, BA="+areaBasal.ToString("R",CultureInfo.InvariantCulture)+" m2, initial removals="+a.PlannedRemovals+" BA share="+a.RemovedBasalShare.ToString("R",CultureInfo.InvariantCulture)+" min="+a.MinPlannedAndExecuted.ToString("R",CultureInfo.InvariantCulture));
        Check(a.ReleasePlanned&&a.PlannedRemovals>=10&&a.CanopyKept,"30% initial calibrated release remains feasible and substantial inside footprint");
        Check(!a.MixedPlanned&&!a.ReleaseExecuted,"planned all-sold work is neither executed nor mixed");
        string wood=release.Orders.First(o=>o.Open).TargetId;release.Plan(wood,ScenarioOneRackBay.Disposition.Deadwood);a=release.Analyse();
        Check(a.MixedPlanned&&a.DeadwoodPlanned==1,"inside-footprint sold and deadwood choices counted per removal");
        Check(ScenarioOneTeachingSequence.Current(Facts(a))==3,"adequate initial plan advances to carrying out intervention");
        release.Execute(2);a=release.Analyse();
        Check(a.ReleaseExecuted&&a.MixedExecuted&&a.ReleaseYear==2,"initial intervention executed with both outcomes");
        Check(ScenarioOneTeachingSequence.Current(Facts(a,true))==4,"annual acknowledgement alone cannot finish Objective 5");
        var lost=release.Clone();int li=lost.Trees.FindIndex(t=>t.Id==Future[0]);var lt=lost.Trees[li];lost.Trees[li]=new ScenarioOneRackBay.Tree(lt.Id,lt.X,lt.Z,lt.DbhCm,true,false);
        Check(!lost.Analyse().HasEnoughFuture&&ScenarioOneTeachingSequence.Current(Facts(lost.Analyse(),true,true))==1,"losing a selection cannot bypass the three-tree final boundary");
        // Every in-footprint removal enters safeguard, including trees not competing with a selected tree.
        var over=selected.Clone();foreach(var t in over.Trees.Where(t=>t.Living&&!t.Crop&&a.Contains(t.X,t.Z)).ToList())over.Plan(t.Id);
        Check(!over.Analyse().CanopyKept&&!over.Analyse().ReleasePlanned,"over-cap planning fails");
        over.Execute(2);var exceeded=over.Analyse();
        Check(!exceeded.CanopyKept&&!exceeded.ReleaseExecuted&&ScenarioOneTeachingSequence.Current(Facts(exceeded,true,true))<5,"over-cap executed stand cannot complete even with injected review facts");
        var review=new ScenarioOneRackReview();int serial=10;
        review.Synchronize(a,false,serial);var first=a.Future[0];review.Observe(a,first.Id,first.X,first.Z,++serial);
        Check(!review.Decide(a,first.Id,first.X,first.Z,ScenarioOneRackReview.Decision.RetainHere),"pre-acknowledgement inspection is not later-stage review");
        review.Synchronize(a,true,serial);review.Observe(a,first.Id,first.X,first.Z,serial);
        Check(review.InspectedCount==0&&!review.Complete(a),"keeping inspection open through review creates no revisit");
        Check(!review.Decide(a,first.Id,first.X,first.Z,ScenarioOneRackReview.Decision.RetainHere),"generic decision without a real new inspection fails");
        foreach(var f in a.Future.Take(2)){review.Observe(a,f.Id,f.X,f.Z,++serial);review.Decide(a,f.Id,f.X,f.Z,ScenarioOneRackReview.Decision.RetainHere);}
        Check(review.InspectedCount==2&&!review.Complete(a),"two inspected trees do not complete Objective 5");
        var third=a.Future[2];review.Observe(a,third.Id,third.X,third.Z,++serial);review.Decide(a,third.Id,third.X,third.Z,ScenarioOneRackReview.Decision.RetainHere);
        Check(review.InspectedCount==3&&!review.Complete(a),"three tree decisions still need surrounding retained-stand review");
        ReviewAll(release,review,ref serial);
        Check(review.Complete(a)&&review.StandPortionsReviewed==6,"retain-here decisions can complete with zero extra felling");
        var complete=Facts(a,true,review.Complete(a));complete.ReviewedTwoCycles=false;
        Check(ScenarioOneTeachingSequence.Current(complete)==5,"no forced extra management cycle or 40% release quota in rack lesson");
        review.Reset(serial);review.Synchronize(a,true,serial);
        Check(!review.Complete(a)&&review.DecisionCount==0,"load/new-world/session reset cannot fabricate inspection evidence");
        ReviewAll(release,review,ref serial);
        // Additional release remains a real player option, not a compulsory number.
        var more=release.Clone();var f0=a.Future[0];var ft=more.Get(f0.Id);
        var extra=more.Trees.Where(t=>t.Living&&!t.Crop&&a.Contains(t.X,t.Z)&&Dist(t,ft)<=8).OrderBy(t=>t.DbhCm).First();
        more.Mark(extra.Id);var ma=more.Analyse();
        Check(ma.CanopyKept,"a small justified additional release fits safeguard");
        foreach(var f in ma.Future.Where(f=>f.MarkedShare+f.PlannedShare>0))
        {review.Observe(ma,f.Id,f.X,f.Z,++serial);Check(review.Decide(ma,f.Id,f.X,f.Z,ScenarioOneRackReview.Decision.ReleaseFurther),"further-release decision records a relevant marked competitor");}
        Check(review.Complete(ma),"additional release possible without mandatory percentage increase");
        more.Plan(extra.Id);more.Execute(3);var after=more.Analyse();review.Synchronize(after,true,serial);
        Check(!review.Complete(after)&&review.InspectedCount==0,"further executed work invalidates earlier inspection decisions");
        review.Synchronize(after,false,serial);Check(!review.Complete(after),"latest executed results need acknowledgement before return review");
        review.Synchronize(after,true,serial);ReviewAll(more,review,ref serial);Check(review.Complete(after),"further intervention can be reviewed and retained successfully");
        var denied=new ScenarioOneRackReview();denied.Synchronize(a,true,serial);denied.Observe(a,first.Id,first.X,first.Z,++serial);
        Check(!denied.Decide(a,first.Id,first.X,first.Z,ScenarioOneRackReview.Decision.ReleaseFurther),"release-further choice needs an actual relevant mark/order");
        var reorder=release.Clone();reorder.Trees.Reverse();reorder.Orders.Reverse();var ra=reorder.Analyse();
        Check(ra.HasEnoughFuture&&Math.Abs(ra.RemovedBasalShare-a.RemovedBasalShare)<1e-6&&ra.ReleaseKey==a.ReleaseKey,"listing order does not change the lesson");
        string copy=string.Join(" ",ScenarioOneTeachingSequence.Lessons.Skip(1).Take(4).Select(l=>l.Title+l.Concept+l.Action+l.Practice));
        Check(!copy.ToLowerInvariant().Contains("overtopped")&&!copy.Contains("two fifths")&&copy.Contains("deciding not to cut"),"accepted objective copy removes unsupported overtopping and forced later quota");
        Check(ScenarioOneRackBayText.Progress(4,Facts(a,true)).Contains("Revisited"),"Objective 5 reports inspections and decisions, not another percentage");
        Check(ScenarioOneTeachingSequence.Lessons.Length==8&&ScenarioOneTeachingSequence.Lessons[0].Id=="access"&&ScenarioOneTeachingSequence.Lessons[5].Id=="recruitment","Objective 1 and 6+ identities retained");
        Console.WriteLine(failures==0?$"NONUI_RACKBAY_PASS checks={checks}":$"NONUI_RACKBAY_FAIL failures={failures} checks={checks}");return failures==0?0:1;
    }
}
