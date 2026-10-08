using System;
using System.Linq;
using UnityEngine;

public sealed partial class ScenarioOneManager
{
    // Not saved; task trial parameters. Saves persist the five approved state
    // fields and model id, never a mutable calibration profile.
    public UnderstoreyCompetitionCalibration CompetitionCalibration { get; set; } = new UnderstoreyCompetitionCalibration();
    public PlantedCompetitionAnnualAccount PlantedCompetitionAccount { get; private set; } = new PlantedCompetitionAnnualAccount();
    private CompetitionExposureCache competitionExposure;
    public CompetitionExposureCache CompetitionCache => competitionExposure;
    private void InvalidateCompetition() => competitionExposure=null;

    public void PrepareCompetitionForYear(int year)
    {
        if(ecology.RegenerationModelVersion!=RegenerationModel.Competition)return;
        EnsureUnderstoreyGrid();
        // Model 2 advances habitat AND competitor state here, before juvenile
        // survival. Model 0/1 retain their old post-juvenile habitat update.
        for(int i=0;i<understoreyCells.Count;i++)
        {
            var state=understoreyCells[i];
            if(year<=state.lastUpdatedYear)continue;
            float target=UnderstoreyCompetition.Target(ecology.Cells[i],CompetitionCalibration);
            state.brambleCover=UnderstoreyCompetition.Step(state.brambleCover,target,CompetitionCalibration);
            state.brackenCover=UnderstoreyCompetition.Step(state.brackenCover,target,CompetitionCalibration);
            ScenarioOneUnderstorey.Advance(state,ecology.Cells[i],year,definition.UnderstoreyColonisationRate,definition.UnderstoreyLossRate);
        }
        foreach(var patch in clearancePatches)
        {
            if(year<=patch.competitionUpdatedYear)continue;
            int index=ecology.GetCellIndex(patch.center);
            if(index<0)throw new InvalidOperationException("Competitor patch outside ecology grid");
            float target=UnderstoreyCompetition.Target(ecology.Cells[index],CompetitionCalibration);
            patch.brambleCover=UnderstoreyCompetition.Step(patch.brambleCover,target,CompetitionCalibration);
            patch.brackenCover=UnderstoreyCompetition.Step(patch.brackenCover,target,CompetitionCalibration);
            patch.competitionUpdatedYear=year;
        }
        RebuildCompetitionExposure();
    }
    public void RebuildCompetitionExposure()
    {
        EnsureUnderstoreyGrid();
        var areaYear=new int[ecology.CellCount];
        foreach(var order in workOrders)
            if(order.type==ScenarioWorkType.RemoveRegeneration&&string.IsNullOrEmpty(order.speciesId)&&order.status==ScenarioWorkStatus.Completed&&order.cellIndex>=0&&order.cellIndex<areaYear.Length)
                areaYear[order.cellIndex]=Math.Max(areaYear[order.cellIndex],order.resolvedYear);
        competitionExposure=new CompetitionExposureCache(ecology,understoreyCells,clearancePatches,areaYear);
    }
    public float CompetitionCellExposure(int index)
    {
        if(competitionExposure==null)RebuildCompetitionExposure();
        return competitionExposure.CellPressure[index];
    }
    public Vector2 CompetitionCoversAt(Vector3 position)
    {
        if(competitionExposure==null)RebuildCompetitionExposure();
        int index=ecology.GetCellIndex(position);
        return index<0?Vector2.zero:competitionExposure.PointCovers(index,position);
    }
    public float CompetitionExposureAt(Vector3 position)
    {
        Vector2 cover=CompetitionCoversAt(position);return Mathf.Max(cover.x,cover.y);
    }
    private void ResetCellCompetition(int index,int year)
    {
        EnsureUnderstoreyGrid();var state=understoreyCells[index];
        state.brambleCover=state.brackenCover=0f;state.lastUpdatedYear=year;
        // The completed, already-saved area work order masks older local
        // overrides only within this cell. Cross-border state is not erased.
        // Same-year planting patches already start at zero.
    }
}
