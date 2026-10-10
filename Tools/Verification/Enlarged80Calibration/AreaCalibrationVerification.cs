#if UNITY_EDITOR
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

// Disposable gate: no saved files or serialized assets are modified.
public static class AreaCalibrationVerification
{
    public static void Begin() { EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity"); EditorApplication.isPlaying = true; }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install() { if (Environment.GetCommandLineArgs().Contains("AreaCalibrationVerification.Begin")) new GameObject("Calibration gate").AddComponent<AreaCalibrationRunner>(); }
}
public sealed class AreaCalibrationRunner : MonoBehaviour
{
    static void Check(bool ok, string reason) { if (!ok) throw new InvalidOperationException(reason); }
    IEnumerator Start()
    {
        for (int i=0; i<5; i++) yield return null;
        try { Verify(); Debug.Log("AREA_CALIBRATION_VERIFY_PASS"); }
        catch (Exception e) { Debug.LogError("AREA_CALIBRATION_VERIFY_FAIL " + e); EditorApplication.Exit(1); yield break; }
        EditorApplication.Exit(0);
    }
    void Verify()
    {
        var manager=FindFirstObjectByType<ScenarioOneManager>(); var d=manager.Definition;
        foreach (int geometry in new[]{StandGeometryModel.Legacy40,StandGeometryModel.Enlarged80})
        {
            int k=geometry==StandGeometryModel.Legacy40?1:4;
            Check(d.EffectiveMinimumRetainedOriginalTrees(geometry)==60*k,"retained");
            Check(d.EffectiveMinimumRegenerationCells(geometry)==3*k,"regeneration");
            Check(d.EffectiveMinimumDeadwoodVolumeM3(geometry)==0.02f*k,"deadwood");
            Check(d.EffectiveReferenceOriginalTrees(geometry)==120*k,"century trees");
            Check(d.EffectiveReferenceBroadleafPresence(geometry)==10*k,"century broadleaf");
            Check(d.EffectiveReferenceRegenerationCells(geometry)==12*k,"century regeneration");
            Check(d.EffectiveReferenceDeadwoodVolumeM3(geometry)==0.5f*k,"century deadwood");
            Check(d.MinimumMeanCanopy==0.35f && d.ReferenceMeanCanopy==0.65f && d.MinimumCompletionYear==25 && d.CenturyReviewYear==100,"unscaled means/time");
            Check(d.StartingCashCents==1200000 && d.MinimumHarvestJobCents==250000 && d.OwnerMinutesPerYear==2400,"unscaled resources");
            var snapshot=new ScenarioEcologicalSnapshot { year=25, meanCanopy=0.35f, occupiedRegenerationCells=3*k, deadwoodVolumeM3=0.02f*k };
            snapshot.species.Add(new ScenarioSpeciesOutcome {speciesId="sitka-spruce",livingTrees=60*k});
            var objectives=ScenarioOneObjectives.Evaluate(d,snapshot,null,"sitka-spruce",geometry:geometry);
            foreach(var pair in new[]{("retained-canopy",60f*k),("regeneration",3f*k),("fallen-deadwood",0.02f*k)})
            { var o=objectives.Single(x=>x.objectiveId==pair.Item1); Check(o.targetValue==pair.Item2 && o.achieved,"evaluated target "+pair.Item1); Check(ScenarioOneUiFacts.ObjectiveLine(o).Contains("/"),"shared objective formatting"); }
            var review=ScenarioOneObjectives.Review(d,snapshot,ScenarioOneOutcome.Active,-1,"sitka-spruce",geometry:geometry);
            Check(review.referenceId=="aspirational-design-targets","fallback ID");
            foreach(var pair in new[]{("original-trees",120f*k),("reference-regeneration",12f*k),("reference-deadwood",0.5f*k),("reference-beech",10f*k),("reference-sessile-oak",10f*k)})
                Check(review.referenceComparisons.Single(x=>x.objectiveId==pair.Item1).targetValue==pair.Item2,"review target "+pair.Item1);
            string copy=ScenarioOneUiFacts.CenturyComparisonDescription(review);
            Check(copy.Contains("aspirational design targets") && copy.Contains("not a forecast, optimum or prescription") && !copy.Contains("frozen Reference"),"fallback copy");
            Check(ScenarioOneUiFacts.CenturyComparisonValueLabel(review)=="target","target label");
            Debug.Log("AREA_CALIBRATION_VALUES "+StandGeometryModel.Label(geometry)+" retained="+60*k+" regen="+3*k+" deadwood="+0.02f*k+" century="+120*k+"/"+10*k+"/"+12*k+"/"+0.5f*k);
        }
        var ecology=FindFirstObjectByType<ForestEcologyController>();
        Check(manager.Objectives.Single(x=>x.objectiveId=="retained-canopy").targetValue==d.EffectiveMinimumRetainedOriginalTrees(ecology.StandGeometryModelVersion),"manager UI target source");
        // Restored pre-calibration E80 reviews must use the same current target layer.
        var original=manager.CaptureSaveData();
        var cached=manager.CaptureSaveData();
        var century=new ScenarioEcologicalSnapshot {year=100};
        century.species.Add(new ScenarioSpeciesOutcome {speciesId="sitka-spruce",livingTrees=200});
        cached.centuryReview=ScenarioOneObjectives.Review(d,century,ScenarioOneOutcome.Active,-1,"sitka-spruce",geometry:StandGeometryModel.Legacy40);
        string savedReview=JsonUtility.ToJson(cached.centuryReview);
        manager.RestoreSaveData(cached,20);
        Check(manager.CenturyReview.referenceComparisons.Single(x=>x.objectiveId=="original-trees").targetValue==480,"restored E80 century targets");
        Check(manager.CenturyReview.referenceComparisons.Single(x=>x.objectiveId=="reference-deadwood").targetValue==2f,"restored E80 deadwood target");
        Check(JsonUtility.ToJson(cached.centuryReview)==savedReview,"restore does not mutate input record");
        Check(manager.CenturyReview.year==100 && manager.CenturyReview.outcome==ScenarioOneOutcome.Active
            && manager.CenturyReview.referenceComparisons.Single(x=>x.objectiveId=="original-trees").currentValue==200
            && !manager.CenturyReview.referenceComparisons.Single(x=>x.objectiveId=="original-trees").achieved,"cached measurements/history preserved, achieved recalibrated");
        foreach(var pair in new[]{("reference-regeneration",48f),("reference-beech",40f),("reference-sessile-oak",40f)})
            Check(manager.CenturyReview.referenceComparisons.Single(x=>x.objectiveId==pair.Item1).targetValue==pair.Item2,"restored target "+pair.Item1);
        var legacyCopy=JsonUtility.FromJson<ScenarioCenturyReview>(savedReview);
        ScenarioOneObjectives.RefreshAspirationalTargets(legacyCopy,d,StandGeometryModel.Legacy40);
        Check(JsonUtility.ToJson(legacyCopy)==savedReview,"Legacy40 cached review unchanged");
        cached.centuryReview.referenceId=ScenarioReferenceArchive.Load().referenceId;
        string frozenSaved=JsonUtility.ToJson(cached.centuryReview);
        manager.RestoreSaveData(cached,20);
        Check(JsonUtility.ToJson(manager.CenturyReview)==frozenSaved,"frozen cached review unchanged");
        manager.RestoreSaveData(original,20);
        Debug.Log("AREA_CALIBRATION_RESTORED_REVIEW_PASS");
        bool rejected=false; try{d.EffectiveMinimumRetainedOriginalTrees(77);}catch(ArgumentOutOfRangeException){rejected=true;} Check(rejected,"unknown model rejected");
        var frozen=new ScenarioCenturyReview {referenceId=ScenarioReferenceArchive.Load().referenceId};
        Check(ScenarioOneUiFacts.CenturyComparisonDescription(frozen).Contains("frozen Reference Future") && ScenarioOneUiFacts.CenturyComparisonValueLabel(frozen)=="reference","frozen copy");
        Check(ForestSaveData.CurrentVersion==20 && d.DefinitionVersion=="scenario-one-v13","unchanged identities");
    }
}
#endif
