using System;
using System.Collections.Generic;
using UnityEngine;

// Runtime trial parameters, not extra saved ecological state. All numeric
// response/target/window choices are [C]; gain/loss begin as [D] candidates.
public sealed class UnderstoreyCompetitionCalibration
{
    public float Strength = .35f;
    public float FullHeight = .3f;
    public float EscapeHeight = 1.5f;
    public float Gain = .30f;
    public float Loss = .45f;
    public float InitialFraction = .25f;
    public float TargetMaximum = .8f;
    public float ShadeTargetFraction = .1f;
}

public static class UnderstoreyCompetition
{
    // [I] This Scenario One site has potential bramble/bracken populations.
    // Shared target avoids inventing a type ranking. Deep shade is not an
    // absence rule; .1 shade floor and light ramp are [C], not field rates.
    public static float Target(ForestEcologyCell cell, UnderstoreyCompetitionCalibration c)
    {
        float site = Mathf.Clamp01(cell.SiteProductivity * cell.SoilStability);
        float light = Mathf.Clamp01(cell.Light);
        return c.TargetMaximum * site * (c.ShadeTargetFraction + (1f-c.ShadeTargetFraction)*light);
    }
    public static void Initialize(ScenarioUnderstoreyCell state, ForestEcologyCell cell, UnderstoreyCompetitionCalibration c)
    {
        state.brambleCover = state.brackenCover = Target(cell,c)*c.InitialFraction;
    }
    public static float Step(float cover, float target, UnderstoreyCompetitionCalibration c)
        => Mathf.Clamp01(Mathf.Lerp(cover,target,target>cover?c.Gain:c.Loss));
    public static float LossProbability(float exposure, float height, UnderstoreyCompetitionCalibration c)
        => Mathf.Clamp01(c.Strength * Mathf.Clamp01(exposure) * Mathf.Clamp01((c.EscapeHeight-height)/(c.EscapeHeight-c.FullHeight)));
}

public sealed class PlantedCompetitionSpeciesAccount
{
    public int Starting, VegetationDeaths, LightDeaths, BrowseDeaths, Remaining, Promoted;
}
public sealed class PlantedCompetitionAnnualAccount
{
    public int Year;
    public readonly SortedDictionary<string,PlantedCompetitionSpeciesAccount> Species = new SortedDictionary<string,PlantedCompetitionSpeciesAccount>(StringComparer.Ordinal);
    public PlantedCompetitionSpeciesAccount For(string id)
    {
        if (!Species.TryGetValue(id,out var account)) Species[id]=account=new PlantedCompetitionSpeciesAccount();
        return account;
    }
}

// Build once per annual step. Cells have local patch buckets. Weighted
// strip integration sweeps interval events with an ordered active set:
// overlapping circle area is assigned once to the highest-precedence patch.
// Exact-point queries use a balanced bounding tree with priority pruning,
// rather than a juvenile x global-patch nested scan.
public sealed class CompetitionExposureCache
{
    private sealed class Patch { public PlantingClearancePatch State; public int Rank; }
    private sealed class Node
    {
        public Rect Bounds; public Vector2 CenterMin, CenterMax; public float MaxRadius; public int MaxRank; public Patch Leaf; public Node Left,Right;
    }
    private struct Event { public float Y; public int Rank; public bool Enter; }
    private readonly List<Patch>[] buckets;
    private readonly Node[] roots;
    private readonly IReadOnlyList<ScenarioUnderstoreyCell> cells;
    public readonly float[] CellPressure, MeanBramble, MeanBracken;
    public int PointNodesVisited { get; private set; }
    public int PatchCellAssignments { get; private set; }
    public CompetitionExposureCache(ForestEcologyController ecology,IReadOnlyList<ScenarioUnderstoreyCell> state,
        IReadOnlyList<PlantingClearancePatch> source,int[] areaClearYear)
    {
        cells=state; int n=ecology.CellCount;
        buckets=new List<Patch>[n];roots=new Node[n];CellPressure=new float[n];MeanBramble=new float[n];MeanBracken=new float[n];
        for(int i=0;i<n;i++)buckets[i]=new List<Patch>();
        var sorted=new List<PlantingClearancePatch>(source);
        sorted.Sort(Compare);
        Rect stand=ecology.StandBounds; float size=ecology.CellSizeMeters;int side=ecology.CellsPerAxis;
        for(int rank=0;rank<sorted.Count;rank++)
        {
            var p=sorted[rank];float r=p.radiusMeters;
            int x0=Mathf.Max(0,Mathf.FloorToInt((p.center.x-r-stand.xMin)/size));
            int x1=Mathf.Min(side-1,Mathf.FloorToInt((p.center.x+r-stand.xMin)/size));
            int z0=Mathf.Max(0,Mathf.FloorToInt((p.center.z-r-stand.yMin)/size));
            int z1=Mathf.Min(side-1,Mathf.FloorToInt((p.center.z+r-stand.yMin)/size));
            for(int z=z0;z<=z1;z++)for(int x=x0;x<=x1;x++)
            {
                int index=z*side+x;
                // A later cell treatment supersedes only the portion in this
                // cell. Old cross-border patches retain state outside it.
                if(p.createdYear<areaClearYear[index])continue;
                buckets[index].Add(new Patch{State=p,Rank=rank});PatchCellAssignments++;
            }
        }
        for(int i=0;i<n;i++)
        {
            Weighted(i,ecology.Cells[i].Center,size*.5f);
            roots[i]=Build(new List<Patch>(buckets[i]),0);
        }
    }
    public static int Compare(PlantingClearancePatch a,PlantingClearancePatch b)
    {
        int v=a.createdYear.CompareTo(b.createdYear);if(v!=0)return v;
        v=a.center.x.CompareTo(b.center.x);if(v!=0)return v;
        v=a.center.z.CompareTo(b.center.z);if(v!=0)return v;
        return a.radiusMeters.CompareTo(b.radiusMeters);
    }
    private static Rect Bounds(Patch p)=>new Rect(p.State.center.x-p.State.radiusMeters,p.State.center.z-p.State.radiusMeters,2*p.State.radiusMeters,2*p.State.radiusMeters);
    private static Node Build(List<Patch> patches,int depth)
    {
        if(patches.Count==0)return null;
        if(patches.Count==1)
        {
            var patch=patches[0];var center=new Vector2(patch.State.center.x,patch.State.center.z);
            return new Node{Leaf=patch,Bounds=Bounds(patch),MaxRank=patch.Rank,CenterMin=center,CenterMax=center,MaxRadius=patch.State.radiusMeters};
        }
        patches.Sort((a,b)=>depth%2==0?a.State.center.x.CompareTo(b.State.center.x):a.State.center.z.CompareTo(b.State.center.z));
        int middle=patches.Count/2;var left=Build(patches.GetRange(0,middle),depth+1);var right=Build(patches.GetRange(middle,patches.Count-middle),depth+1);
        return new Node{Left=left,Right=right,MaxRank=Math.Max(left.MaxRank,right.MaxRank),
            CenterMin=Vector2.Min(left.CenterMin,right.CenterMin),CenterMax=Vector2.Max(left.CenterMax,right.CenterMax),MaxRadius=Mathf.Max(left.MaxRadius,right.MaxRadius),
            Bounds=Rect.MinMaxRect(Mathf.Min(left.Bounds.xMin,right.Bounds.xMin),Mathf.Min(left.Bounds.yMin,right.Bounds.yMin),Mathf.Max(left.Bounds.xMax,right.Bounds.xMax),Mathf.Max(left.Bounds.yMax,right.Bounds.yMax))};
    }
    private void Query(Node node,Vector2 point,ref Patch winner)
    {
        if(node==null||winner!=null&&node.MaxRank<=winner.Rank||point.x<node.Bounds.xMin||point.x>node.Bounds.xMax||point.y<node.Bounds.yMin||point.y>node.Bounds.yMax)return;
        // Distance to the rectangle of patch centers is a lower bound on
        // distance to every center. This rejects points outside every circle,
        // including overlapping bounding-box corners, without changing winners.
        var nearest=new Vector2(Mathf.Clamp(point.x,node.CenterMin.x,node.CenterMax.x),Mathf.Clamp(point.y,node.CenterMin.y,node.CenterMax.y));
        if((point-nearest).sqrMagnitude>node.MaxRadius*node.MaxRadius)return;
        PointNodesVisited++;
        if(node.Leaf!=null)
        {
            var p=node.Leaf.State;
            if((point-new Vector2(p.center.x,p.center.z)).sqrMagnitude<=p.radiusMeters*p.radiusMeters)winner=node.Leaf;
            return;
        }
        Node first=node.Left.MaxRank>node.Right.MaxRank?node.Left:node.Right;
        Query(first,point,ref winner);Query(first==node.Left?node.Right:node.Left,point,ref winner);
    }
    public Vector2 PointCovers(int index,Vector3 position)
    {
        Patch winner=null;Query(roots[index],new Vector2(position.x,position.z),ref winner);
        return winner==null?new Vector2(cells[index].brambleCover,cells[index].brackenCover):new Vector2(winner.State.brambleCover,winner.State.brackenCover);
    }
    private void Weighted(int index,Vector2 centre,float half)
    {
        var background=cells[index];var patches=buckets[index];
        if(patches.Count==0){MeanBramble[index]=background.brambleCover;MeanBracken[index]=background.brackenCover;CellPressure[index]=Mathf.Max(background.brambleCover,background.brackenCover);return;}
        var rankState=new Dictionary<int,PlantingClearancePatch>();foreach(var p in patches)rankState[p.Rank]=p.State;
        var events=new List<Event>();var active=new SortedSet<int>();
        int strips=Mathf.CeilToInt(half*2f/.02f);float dx=half*2f/strips;
        double b=0,k=0,pressure=0;float bottom=centre.y-half,top=centre.y+half;
        for(int strip=0;strip<strips;strip++)
        {
            float x=centre.x-half+(strip+.5f)*dx;events.Clear();active.Clear();
            foreach(var patch in patches)
            {
                var p=patch.State;float offset=x-p.center.x;
                if(Mathf.Abs(offset)>=p.radiusMeters)continue;
                float h=Mathf.Sqrt(p.radiusMeters*p.radiusMeters-offset*offset);
                float low=Mathf.Max(bottom,p.center.z-h),high=Mathf.Min(top,p.center.z+h);
                if(high<=low)continue;
                events.Add(new Event{Y=low,Rank=patch.Rank,Enter=true});events.Add(new Event{Y=high,Rank=patch.Rank,Enter=false});
            }
            events.Sort((a,c)=>a.Y.CompareTo(c.Y));float previous=bottom;int next=0;
            while(next<events.Count)
            {
                float y=events[next].Y;Accumulate(y-previous);
                while(next<events.Count&&events[next].Y==y){var change=events[next++];if(change.Enter)active.Add(change.Rank);else active.Remove(change.Rank);}
                previous=y;
            }
            Accumulate(top-previous);
            void Accumulate(float length)
            {
                float bc=background.brambleCover,kc=background.brackenCover;
                if(active.Count>0){var p=rankState[active.Max];bc=p.brambleCover;kc=p.brackenCover;}
                b+=length*dx*bc;k+=length*dx*kc;pressure+=length*dx*Mathf.Max(bc,kc);
            }
        }
        float area=half*half*4f;MeanBramble[index]=(float)(b/area);MeanBracken[index]=(float)(k/area);CellPressure[index]=(float)(pressure/area);
    }
}
