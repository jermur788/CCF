// Expected cash after this year's planned work, and whether it would fall
// below the contractor's minimum harvesting charge.
//
// Why that threshold matters in Scenario One (ScenarioOneManager and
// ScenarioOneEconomyAdapter): timber sales are the only income; harvesting is
// contractor-only; every harvest costs at least the minimum job charge; and
// work is approved only when cash covers its full cost (sale revenue arrives
// afterwards, at settlement). So once cash is below the minimum, no further
// harvest can be commissioned and no income can follow. Planting, pruning and
// clearance have no minimum, but they only spend money.
public readonly struct CashOutlookInput
{
    public readonly long Cash;
    public readonly long MinimumHarvestCharge;
    // Harvest quotes include the minimum charge; revenue is timber sales only.
    public readonly long ApprovedHarvestCost, ApprovedHarvestRevenue;
    public readonly long AllHarvestCost, AllHarvestRevenue;   // approved + pending
    public readonly long ApprovedOtherCost, PendingOtherCost; // planting, pruning, clearance

    public CashOutlookInput(long cash, long minimumHarvestCharge, long approvedHarvestCost, long approvedHarvestRevenue,
        long allHarvestCost, long allHarvestRevenue, long approvedOtherCost, long pendingOtherCost)
    {
        Cash = cash;
        MinimumHarvestCharge = minimumHarvestCharge;
        ApprovedHarvestCost = approvedHarvestCost;
        ApprovedHarvestRevenue = approvedHarvestRevenue;
        AllHarvestCost = allHarvestCost;
        AllHarvestRevenue = allHarvestRevenue;
        ApprovedOtherCost = approvedOtherCost;
        PendingOtherCost = pendingOtherCost;
    }
}

public enum CashOutlookState
{
    Comfortable,
    // Approving the pending work would leave expected cash below the minimum.
    PendingCrossesMinimum,
    // Already-approved work alone leaves expected cash below the minimum.
    ApprovedCrossesMinimum,
    // Cash is already below the minimum and no approved harvest will bring timber income.
    // (A harvest cannot be approved now, because its cost is at least the minimum.)
    BelowMinimumNow
}

public sealed class CashOutlook
{
    public long ExpectedAfterApproved;
    public long ExpectedIfAllApproved;
    public bool HasPendingWork;
    public CashOutlookState State;
    // Cash that could still be spent before expected cash after all planned
    // work falls below the minimum harvesting charge (0 when already below).
    public long Headroom;

    public static CashOutlook Evaluate(CashOutlookInput input)
    {
        var outlook = new CashOutlook
        {
            ExpectedAfterApproved = input.Cash - input.ApprovedOtherCost - input.ApprovedHarvestCost + input.ApprovedHarvestRevenue,
            ExpectedIfAllApproved = input.Cash - input.ApprovedOtherCost - input.PendingOtherCost - input.AllHarvestCost + input.AllHarvestRevenue,
            HasPendingWork = input.PendingOtherCost > 0 || input.AllHarvestCost != input.ApprovedHarvestCost
                || input.AllHarvestRevenue != input.ApprovedHarvestRevenue
        };
        long minimum = input.MinimumHarvestCharge;
        outlook.Headroom = System.Math.Max(0L, outlook.ExpectedIfAllApproved - minimum);
        if (minimum <= 0)
            outlook.State = CashOutlookState.Comfortable;
        else if (input.Cash < minimum && input.ApprovedHarvestCost == 0)
            outlook.State = CashOutlookState.BelowMinimumNow;
        else if (outlook.ExpectedAfterApproved < minimum)
            outlook.State = CashOutlookState.ApprovedCrossesMinimum;
        else if (outlook.HasPendingWork && outlook.ExpectedIfAllApproved < minimum)
            outlook.State = CashOutlookState.PendingCrossesMinimum;
        else
            outlook.State = CashOutlookState.Comfortable;
        return outlook;
    }
}
